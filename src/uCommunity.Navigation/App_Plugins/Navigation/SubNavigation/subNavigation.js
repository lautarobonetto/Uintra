﻿import appInitializer from "./../../Core/Content/scripts/AppInitializer";

require("./subNavigation.css");

var mobileMediaQuery = window.matchMedia("(max-width: 899px)");
var menu = $('.tabset__navigation');
var title = $('.tabset__title');
var menuHolder = menu.closest('.tabset');
var body = $('body');

if (menu.length > 0) {
    body.addClass('_with-sub-nav');
}

if (title.length > 0) {
    body.addClass('_with-subtitle');
}

/*var initSubMenuPosition = function () {
    var submenu = $('div.tabset .tabset__navigation');
    var holder = title.closest('.tabset__holder');

    if (title.length > 0 && submenu.length > 0) {
        submenu.closest('.tabset').remove();
        submenu.appendTo(holder);
    }
}*/
    
var initMobileMenu = function() {
    var opener = menu.find('.tabset__navigation-link');

    if (menu.find('._active').length <= 0) {
        menu.find('.tabset__navigation-item:first-child').addClass('_active');
    }

    if (opener.length > 1) {
        opener.on("click", function (e) {
            if ($(this).closest('._active').length > 0) {
                e.preventDefault();
                menuHolder.toggleClass('_expanded');
                body.removeClass('_search-expanded notifications-expanded _menu-expanded _sidebar-expanded');
            }
        });
    }
    else {
        opener.closest('.tabset__navigation-item').addClass('_disabled');
    }
}

var controller = {
    init: function () {
        if (mobileMediaQuery.matches) {
            initMobileMenu();
        }

        //initSubMenuPosition();
    }
}

appInitializer.add(controller.init);