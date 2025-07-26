

(function($) {
    "use strict";

    // Wait for document to be ready
    $(document).ready(function() {
        
        // Initialize Slick1 (Main slider)
        try {
            if ($('.wrap-slick1').length > 0 && typeof $.fn.slick !== 'undefined') {
                $('.wrap-slick1').each(function(){
                    var wrapSlick1 = $(this);
                    var slick1 = wrapSlick1.find('.slick1');
                    
                    if (slick1.length > 0) {
                        slick1.slick({
                            slidesToShow: 1,
                            slidesToScroll: 1,
                            fade: true,
                            infinite: true,
                            autoplay: true,
                            autoplaySpeed: 6000,
                            arrows: true,
                            appendArrows: wrapSlick1.find('.wrap-slick1-arrows'),
                            prevArrow:'<button class="arrow-slick1 prev-slick1"><i class="fa fa-angle-left" aria-hidden="true"></i></button>',
                            nextArrow:'<button class="arrow-slick1 next-slick1"><i class="fa fa-angle-right" aria-hidden="true"></i></button>',
                            dots: false
                        });
                    }
                });
            }
        } catch (error) {
            console.warn('Slick1 initialization error:', error);
        }

        // Initialize Slick2 (Product sliders)
        try {
            if ($('.wrap-slick2').length > 0 && typeof $.fn.slick !== 'undefined') {
                $('.wrap-slick2').each(function(){
                    var wrapSlick2 = $(this);
                    var slick2 = wrapSlick2.find('.slick2');
                    
                    if (slick2.length > 0) {
                        slick2.slick({
                            slidesToShow: 4,
                            slidesToScroll: 4,
                            infinite: false,
                            autoplay: false,
                            autoplaySpeed: 6000,
                            arrows: true,
                            appendArrows: wrapSlick2,
                            prevArrow:'<button class="arrow-slick2 prev-slick2"><i class="fa fa-angle-left" aria-hidden="true"></i></button>',
                            nextArrow:'<button class="arrow-slick2 next-slick2"><i class="fa fa-angle-right" aria-hidden="true"></i></button>',
                            responsive: [
                                {
                                    breakpoint: 1200,
                                    settings: {
                                        slidesToShow: 4,
                                        slidesToScroll: 4
                                    }
                                },
                                {
                                    breakpoint: 992,
                                    settings: {
                                        slidesToShow: 3,
                                        slidesToScroll: 3
                                    }
                                },
                                {
                                    breakpoint: 768,
                                    settings: {
                                        slidesToShow: 2,
                                        slidesToScroll: 2
                                    }
                                },
                                {
                                    breakpoint: 576,
                                    settings: {
                                        slidesToShow: 1,
                                        slidesToScroll: 1
                                    }
                                }
                            ]
                        });
                    }
                });
            }
        } catch (error) {
            console.warn('Slick2 initialization error:', error);
        }

        // Initialize Slick3 (Gallery sliders)
        try {
            if ($('.wrap-slick3').length > 0 && typeof $.fn.slick !== 'undefined') {
                $('.wrap-slick3').each(function(){
                    var wrapSlick3 = $(this);
                    var slick3 = wrapSlick3.find('.slick3');
                    
                    if (slick3.length > 0) {
                        slick3.slick({
                            slidesToShow: 1,
                            slidesToScroll: 1,
                            fade: true,
                            infinite: true,
                            autoplay: false,
                            autoplaySpeed: 6000,
                            arrows: true,
                            appendArrows: wrapSlick3.find('.wrap-slick3-arrows'),
                            prevArrow:'<button class="arrow-slick3 prev-slick3"><i class="fa fa-angle-left" aria-hidden="true"></i></button>',
                            nextArrow:'<button class="arrow-slick3 next-slick3"><i class="fa fa-angle-right" aria-hidden="true"></i></button>',
                            dots: true,
                            appendDots: wrapSlick3.find('.wrap-slick3-dots'),
                            dotsClass:'slick3-dots',
                            customPaging: function(slick, index) {
                                var portrait = $(slick.$slides[index]).data('thumb');
                                return '<img src=" ' + portrait + ' "/><div class="slick3-dot-overlay"></div>';
                            }
                        });
                    }
                });
            }
        } catch (error) {
            console.warn('Slick3 initialization error:', error);
        }

        // Handle tab changes for Slick2
        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var nameTab = $(e.target).attr('href'); 
            var slick2InTab = $(nameTab).find('.slick2');
            if (slick2InTab.length > 0 && typeof $.fn.slick !== 'undefined') {
                try {
                    slick2InTab.slick('reinit');
                } catch (error) {
                    console.warn('Slick2 reinit error:', error);
                }
            }
        });

        // Modal search functionality
        $('.js-show-modal-search').on('click', function(){
            $('.modal-search-header').addClass('show-modal-search');
            $(this).css('opacity','0');
        });

        $('.js-hide-modal-search').on('click', function(){
            $('.modal-search-header').removeClass('show-modal-search');
            $('.js-show-modal-search').css('opacity','1');
        });

        // Cart functionality
        $('.js-show-cart').on('click', function(){
            $('.js-panel-cart').addClass('show-header-cart');
        });

        $('.js-hide-cart').on('click', function(){
            $('.js-panel-cart').removeClass('show-header-cart');
        });

        // Menu functionality
        $('.js-show-menu').on('click', function(){
            $('.js-panel-menu').addClass('show-header-menu');
        });

        $('.js-hide-menu').on('click', function(){
            $('.js-panel-menu').removeClass('show-header-menu');
        });

        // Filter functionality
        $('.js-show-filter').on('click', function(){
            $('.js-panel-filter').addClass('show-filter');
        });

        $('.js-hide-filter').on('click', function(){
            $('.js-panel-filter').removeClass('show-filter');
        });

    });

})(jQuery);