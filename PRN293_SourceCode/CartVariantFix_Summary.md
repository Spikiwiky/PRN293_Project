# Cart Logic Fixes and Improvements Summary

## Issues Identified and Fixed

### 1. **Variant Pricing Logic** 
**File:** `EcommerceBackend.DataAccess/Repository/CartRepository/CartRepository.cs`
**Issue:** Cart items were always using `product.BasePrice` instead of variant-specific pricing
**Fix:** 
- Added JSON parsing for variant price from `variant.Variants` property
- Now checks if variant exists and parses price from JSON data
- Falls back to `product.BasePrice` if no variant price is found or JSON parsing fails

```csharp
// Calculate price based on variant if available
decimal price = product.BasePrice;
if (variantId.HasValue && product.Variants != null)
{
    var variant = product.Variants.FirstOrDefault(v => v.VariantId == variantId.Value);
    if (variant != null)
    {
        try
        {
            var variantsArray = JsonConvert.DeserializeObject<JArray>(variant.Variants ?? "[]");
            foreach (var variantItem in variantsArray)
            {
                if (variantItem["price"] != null)
                {
                    price = variantItem["price"].Value<decimal>();
                    break; // Use the first price found
                }
            }
        }
        catch (JsonException)
        {
            // If JSON parsing fails, use base price
            price = product.BasePrice;
        }
    }
}
```

### 2. **Stock Validation Logic**
**File:** `EcommerceBackend.BusinessObject/Services/CartService/CartService.cs`
**Issue:** Stock information is stored as JSON in `variant.Variants` property, not as direct properties
**Fix:**
- Restored JSON parsing for stock validation from `variant.Variants` property
- Added proper error handling for JSON parsing failures
- Maintains the original logic that was working correctly

```csharp
// Calculate total stock from variants
var totalStock = 0;
if (product.Variants != null)
{
    foreach (var variant in product.Variants)
    {
        try
        {
            var variantsArray = JsonConvert.DeserializeObject<JArray>(variant.Variants ?? "[]");
            foreach (var variantItem in variantsArray)
            {
                if (variantItem["stock"] != null)
                {
                    totalStock += variantItem["stock"].Value<int>();
                }
            }
        }
        catch (JsonException)
        {
            // If JSON parsing fails, continue with next variant
            continue;
        }
    }
}
```

### 3. **Guest User Foreign Key Constraint**
**File:** `EcommerceBackend.DataAccess/Repository/CartRepository/CartRepository.cs`
**Issue:** Foreign key constraint violation when creating cart for guest user ID 999 that doesn't exist
**Fix:**
- Added automatic guest user creation in `CreateCartAsync` method
- Uses raw SQL with `IDENTITY_INSERT` to create guest user with specific ID
- Ensures guest user exists before creating cart

```csharp
// For guest users (ID 999), we need to ensure the user exists
if (customerId == 999)
{
    var guestUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == 999);
    if (guestUser == null)
    {
        // Use raw SQL to insert with specific ID
        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT [User] ON;
            INSERT INTO [User] ([User_id], [User_name], [Email], [Password], [Role_id], [Status], [Created_at], [Updated_at])
            VALUES (999, 'guest_user', 'guest@example.com', 'guest_password_hash', 3, 1, GETDATE(), GETDATE());
            SET IDENTITY_INSERT [User] OFF;
        ");
    }
}
```

### 4. **Request Validation**
**Files:** 
- `EcommerceBackend.API/Controllers/CartController/CartController.cs`
- `EcommerceFrontend.Web/Controllers/CartController.cs`
**Issue:** Missing input validation for cart operations
**Fix:**
- Added validation for `ProductId > 0` and `Quantity > 0`
- Added Vietnamese error messages for better UX
- Improved error handling and logging

### 5. **Cookie Forwarding**
**File:** `EcommerceFrontend.Web/Controllers/CartController.cs`
**Issue:** Inconsistent cookie forwarding to backend API
**Fix:**
- Improved cookie forwarding logic to send all cookies from request
- Added fallback mechanism for UserId cookie
- Better logging for debugging cookie issues

```csharp
// Forward all cookies from the current request
var cookieHeader = Request.Headers["Cookie"].ToString();
if (!string.IsNullOrEmpty(cookieHeader))
{
    httpRequest.Headers.Add("Cookie", cookieHeader);
}
else
{
    // Fallback: manually add UserId cookie
    var cookieUserId = GetUserIdFromCookies();
    if (cookieUserId.HasValue)
    {
        httpRequest.Headers.Add("Cookie", $"UserId={cookieUserId.Value}");
    }
}
```

### 6. **Error Response Handling**
**Files:** 
- `EcommerceBackend.API/Controllers/CartController/CartController.cs`
- `EcommerceFrontend.Web/Controllers/CartController.cs`
**Issue:** Inconsistent error response formats
**Fix:**
- Standardized error response format
- Added proper JSON parsing for error responses
- Better error messages in Vietnamese

### 7. **Guest User Handling**
**File:** `EcommerceBackend.API/Controllers/CartController/CartController.cs`
**Issue:** Hardcoded guest user ID (999) without proper session management
**Fix:**
- Added validation for guest cart operations
- Improved error handling for guest user scenarios
- Added comments for future session management improvements

### 8. **Frontend Endpoint Selection**
**File:** `EcommerceFrontend.Web/Pages/Products/Index.cshtml`
**Issue:** Frontend was always calling `/api/Cart/add-guest` instead of checking login status
**Fix:**
- Added login status check using `getCookie('UserId')`
- Dynamically selects endpoint based on login status
- Uses `/api/Cart/add` for logged-in users and `/api/Cart/add-guest` for guests

```javascript
// Check if user is logged in
const userId = getCookie('UserId');
const endpoint = userId ? '/api/Cart/add' : '/api/Cart/add-guest';

console.log('User ID:', userId, 'Using endpoint:', endpoint);

// Send data to frontend API (which will proxy to backend)
fetch(endpoint, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    },
    body: JSON.stringify(cartItem)
})
```

### 9. **Entity Framework Translation Issue**
**File:** `EcommerceBackend.DataAccess/Repository/CartRepository/CartRepository.cs`
**Issue:** Entity Framework could not translate `AreVariantAttributesEqual` method to SQL due to JSON parsing
**Fix:**
- Changed from server-side to client-side evaluation using `ToListAsync()`
- Moved JSON comparison logic to client-side after data retrieval
- Maintains the same functionality while avoiding EF translation issues

```csharp
// Before (causing EF translation error)
query = query.Where(cd => AreVariantAttributesEqual(cd.VariantAttributes, normalizedVariantAttributes));

// After (client-side evaluation)
var cartItems = await query.ToListAsync();
return cartItems.Any(cd => AreVariantAttributesEqual(cd.VariantAttributes, normalizedVariantAttributes));
```

### 10. **Variant ID Lookup Logic**
**File:** `EcommerceFrontend.Web/Pages/Products/Index.cshtml`
**Issue:** Variant ID was not being correctly identified when adding products to cart
**Fix:**
- Improved variant ID lookup logic to properly parse the nested JSON structure
- Added detailed console logging for debugging variant matching
- Fixed the iteration through variant groups and individual variants

```javascript
// Find the matching variant
for (let i = 0; i < variantData.length; i++) {
    const variantGroup = variantData[i];
    const variants = JSON.parse(variantGroup.variants);
    
    for (let j = 0; j < variants.length; j++) {
        const variant = variants[j];
        let isMatch = true;
        
        // Check if all selected attributes match this variant
        for (const [key, value] of Object.entries(selectedAttributes)) {
            if (variant[key] !== value) {
                isMatch = false;
                break;
            }
        }
        
        if (isMatch && variant.id) {
            variantId = parseInt(variant.id);
            console.log('Found matching variant ID:', variantId, 'for variant:', variant);
            break;
        }
    }
    
    if (variantId !== null) break;
}
```

### 11. **JSON Parse Error Handling**
**File:** `EcommerceFrontend.Web/Pages/Products/Index.cshtml`
**Issue:** JSON.parse error when trying to parse variant data that might already be an object
**Fix:**
- Added proper error handling for JSON.parse operations
- Check if data is already parsed before attempting to parse again
- Added validation to ensure variants is an array before processing

```javascript
try {
    // Check if variants is already an object or needs parsing
    if (typeof variantGroup.variants === 'string') {
        variants = JSON.parse(variantGroup.variants);
    } else {
        variants = variantGroup.variants;
    }
} catch (parseError) {
    console.error('Error parsing variants:', parseError);
    console.log('Raw variants data:', variantGroup.variants);
    continue;
}

if (!Array.isArray(variants)) {
    console.warn('Variants is not an array:', variants);
    continue;
}
```

### 12. **VariantId Inclusion in Frontend Data**
**File:** `EcommerceFrontend.Web/Pages/Products/Index.cshtml`
**Issue:** VariantId was not included in the variantData sent to frontend, causing null variant IDs
**Fix:**
- Added `VariantId` to the variantData structure in Razor page
- Updated JavaScript logic to use `variantGroup.variantId` instead of `variant.id`
- Added comprehensive debugging to track variant matching process

```csharp
// Before (missing VariantId)
var variantData = product.Variants.Select(v => new
{
    Attributes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(v.Attributes),
    Variants = v.Variants
}).ToList();

// After (including VariantId)
var variantData = product.Variants.Select(v => new
{
    VariantId = v.VariantId,
    Attributes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(v.Attributes),
    Variants = v.Variants
}).ToList();
```

```javascript
// Updated JavaScript logic
if (variantGroup.variantId) {
    variantId = parseInt(variantGroup.variantId);
    console.log('Found matching variant ID:', variantId, 'for variant group:', variantGroup);
}
```

## Key Improvements Made

### 1. **Better Variant Support**
- Proper variant price calculation from JSON data
- Improved variant attribute handling
- Better stock validation for variants using JSON parsing

### 2. **Enhanced Error Handling**
- Comprehensive input validation
- Better error messages in Vietnamese
- Improved logging for debugging

### 3. **Improved Cookie Management**
- More robust cookie forwarding
- Better fallback mechanisms
- Enhanced debugging capabilities

### 4. **Standardized Response Format**
- Consistent success/error response structure
- Better JSON parsing
- Improved frontend-backend communication

### 5. **Guest User Management**
- Automatic guest user creation
- Proper foreign key constraint handling
- Session-based guest cart approach

## Testing Recommendations

1. **Test variant pricing:**
   - Add products with variants to cart
   - Verify correct variant prices are used from JSON data
   - Test with products without variants

2. **Test stock validation:**
   - Try adding more items than available stock
   - Test with products that have no stock
   - Verify variant stock validation from JSON data

3. **Test cookie handling:**
   - Test cart operations when logged in
   - Test guest cart functionality
   - Verify cookie forwarding works correctly

4. **Test guest user functionality:**
   - Test adding items to cart as guest user
   - Verify guest user is created automatically
   - Test guest cart persistence

5. **Test error scenarios:**
   - Invalid product IDs
   - Zero or negative quantities
   - Network errors
   - Backend service unavailability

## Database Setup

### Guest User Creation
Run the SQL script `add_guest_user_alternative.sql` to create the guest user:

```sql
-- Alternative approach: Create guest user with identity insert
IF NOT EXISTS (SELECT 1 FROM [User] WHERE [User_id] = 999)
BEGIN
    SET IDENTITY_INSERT [User] ON;
    
    INSERT INTO [User] (
        [User_id],
        [User_name], 
        [Email], 
        [Password], 
        [Role_id], 
        [Status], 
        [Created_at], 
        [Updated_at]
    ) 
    VALUES (
        999,
        'guest_user', 
        'guest@example.com', 
        'guest_password_hash', 
        3, -- Assuming 3 is the guest role ID
        1, -- Active status
        GETDATE(), 
        GETDATE()
    );
    
    SET IDENTITY_INSERT [User] OFF;
    PRINT 'Guest user created successfully with ID: 999';
END
```

## Future Improvements

1. **Session Management for Guest Users:**
   - Implement proper session-based guest cart management
   - Replace hardcoded guest user ID (999)
   - Add guest cart to user cart conversion on login

2. **Real-time Stock Updates:**
   - Implement real-time stock checking
   - Add stock reservation during checkout
   - Prevent overselling

3. **Cart Persistence:**
   - Implement cart persistence across sessions
   - Add cart backup/restore functionality
   - Improve guest cart handling

4. **Performance Optimization:**
   - Add caching for frequently accessed cart data
   - Optimize database queries
   - Implement pagination for large carts

## Files Modified

1. `EcommerceBackend.DataAccess/Repository/CartRepository/CartRepository.cs`
2. `EcommerceBackend.BusinessObject/Services/CartService/CartService.cs`
3. `EcommerceBackend.API/Controllers/CartController/CartController.cs`
4. `EcommerceFrontend.Web/Controllers/CartController.cs`
5. `EcommerceFrontend.Web/Pages/Products/Index.cshtml`
6. `add_guest_user_alternative.sql` (new file)

## Summary

The cart logic has been significantly improved with better variant support, enhanced error handling, improved cookie management, and standardized response formats. The system now properly handles variant pricing by parsing JSON data from the `Variants` property, validates stock correctly using JSON parsing, and provides better user experience with Vietnamese error messages and robust error handling. The implementation correctly handles the data structure where variant information (price, stock) is stored as JSON rather than direct properties. Additionally, the guest user foreign key constraint issue has been resolved by automatically creating a guest user when needed. 