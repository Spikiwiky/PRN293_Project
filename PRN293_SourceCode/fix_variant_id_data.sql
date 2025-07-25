-- Script to fix VariantId data before migration
-- Run this script before applying the migration

-- Check current data
SELECT 'Cart_detail' as TableName, Cart_detail_id, Variant_id, 
       CASE 
           WHEN ISNUMERIC(Variant_id) = 1 THEN 'Numeric'
           WHEN Variant_id IS NULL THEN 'NULL'
           ELSE 'Non-numeric'
       END as DataType
FROM Cart_detail 
WHERE Variant_id IS NOT NULL
UNION ALL
SELECT 'Order_detail' as TableName, Order_detail_id, Variant_id,
       CASE 
           WHEN ISNUMERIC(Variant_id) = 1 THEN 'Numeric'
           WHEN Variant_id IS NULL THEN 'NULL'
           ELSE 'Non-numeric'
       END as DataType
FROM Order_detail 
WHERE Variant_id IS NOT NULL;

-- Update non-numeric VariantId values to NULL in Cart_detail
UPDATE Cart_detail 
SET Variant_id = NULL 
WHERE Variant_id IS NOT NULL AND ISNUMERIC(Variant_id) = 0;

-- Update empty strings to NULL in Cart_detail
UPDATE Cart_detail 
SET Variant_id = NULL 
WHERE Variant_id = '';

-- Update non-numeric VariantId values to NULL in Order_detail
UPDATE Order_detail 
SET Variant_id = NULL 
WHERE Variant_id IS NOT NULL AND ISNUMERIC(Variant_id) = 0;

-- Update empty strings to NULL in Order_detail
UPDATE Order_detail 
SET Variant_id = NULL 
WHERE Variant_id = '';

-- Verify the data is clean
SELECT 'Cart_detail' as TableName, COUNT(*) as TotalRows,
       COUNT(CASE WHEN Variant_id IS NULL THEN 1 END) as NullVariantId,
       COUNT(CASE WHEN Variant_id IS NOT NULL THEN 1 END) as NonNullVariantId
FROM Cart_detail
UNION ALL
SELECT 'Order_detail' as TableName, COUNT(*) as TotalRows,
       COUNT(CASE WHEN Variant_id IS NULL THEN 1 END) as NullVariantId,
       COUNT(CASE WHEN Variant_id IS NOT NULL THEN 1 END) as NonNullVariantId
FROM Order_detail; 