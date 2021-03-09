"use strict";

import { fadeIn, fadeOut } from "@app/Infra/Util";

$(document).ready(function () {
  $(".navi-menu-button").on("click", function (e) {
    navMenuOpen();
  });

  $(".nav-menu").on("click", function (e) {
    if ($(e.target).hasClass("nav-menu")) {
      navMenuClose();
    }
  });

  $("nav.menu ul.main-menu>li>a").on("click", function (e) {
    var that = $(this);
    e.preventDefault();
    if (!that.parent().hasClass("active")) {
      setTimeout(() => {
        $("nav.menu ul.main-menu > li").removeClass("active");
        $("nav.menu ul li a span")
          .removeClass("fa-angle-up")
          .addClass("fa-angle-down");
        that.parent().addClass("active");
        that.find("span").removeClass("fa-angle-down").addClass("fa-angle-up");
      }, 50);
    } else {
      $(this).parent().removeClass("active");
      that.find("span").removeClass("fa-angle-up").addClass("fa-angle-down");
    }
  });

  $(".tab-item .fix-width .menu-item").css({
    width: 100 / $(".tab-item .fix-width .menu-item").length + "%",
  });

  if ($(".wizard").length) {
    wizardFixHeight();
    $(window).resize();
  }

  if ($(".animated-text").length) animateText();
});

$(".wrapper-inline").on("scroll", function (e) {
  if (this.scrollTop > 150) {
    $("header.no-background").addClass("set-bg");
  } else {
    $("header.no-background").removeClass("set-bg");
  }
});

var navMenuOpen = function () {
  $(".navi-menu-button").addClass("focused");
  fadeIn($("div.nav-menu")[0], 50);
  setTimeout(function () {
    $("nav.menu").addClass("opened");
  }, 50);
};

var navMenuClose = function () {
  $(".navi-menu-button").removeClass("focused");
  $("nav.menu").removeClass("opened");
  fadeOut($("div.nav-menu")[0], 200);
};

var wizardFixHeight = function () {
  $(window).on("resize", function (e) {
    $(".wizard .wizard-item").height($(window).height() - 50);
  });
};

var animateText = function () {
  $(".vertical-center").css({
    "margin-top": $(window).height() / 2 - $(".vertical-center").height() / 2,
  });
  $(".animated-text").removeClass("zero-opacity");
  $("[data-transation]").each(function (e, i) {
    var that = $(this);
    that.addClass("hide");

    var transation = that.attr("data-transation");
    if (transation == "") transation = "fadeInDown";

    var startTime = parseInt(that.attr("data-start-time"));
    if (isNaN(startTime)) startTime = 0;

    setTimeout(function () {
      that.addClass("animated " + transation);
    }, startTime);
  });
};

/*expandable list scrips****/
$(document).on("click", ".expandable-item .expandable-header", function () {
  if ($(this).parent().hasClass("accordion")) {
    if ($(this).parent().hasClass("active")) {
      $(this).parent().removeClass("active");
      $(this).parent().find(".expandable-content").attr("style", "");
    } else {
      var accordionGroup = $(this).parent().attr("data-group");
      $('[data-group="' + accordionGroup + '"]').removeClass("active");
      $('[data-group="' + accordionGroup + '"]')
        .find(".expandable-content")
        .attr("style", "");
      $(this)
        .parent()
        .find(".expandable-content")
        .css({
          "max-height": $(this).parent().find(".expandable-content")[0]
            .scrollHeight,
        });
      $(this).parent().addClass("active");
    }
  } else {
    if ($(this).parent().hasClass("active"))
      $(this).parent().find(".expandable-content").attr("style", "");
    else
      $(this)
        .parent()
        .find(".expandable-content")
        .css({
          "max-height": $(this).parent().find(".expandable-content")[0]
            .scrollHeight,
        });

    $(this).parent().toggleClass("active");
  }
});

$(document).on("click", ".tab-item .menu-item", function (e) {
  e.preventDefault();
  var tabContentId = $(this).attr("data-content");

  $(this).parents(".tab-item").find(".menu-item").removeClass("active");
  $(this).addClass("active");

  $(this).parents(".tab-item").find(".content-item").removeClass("active");
  $("#" + tabContentId).addClass("active");
});

/*post item scripts **************/
$(document).on("click", ".post-item .post-share > i", function (e) {
  e.preventDefault();
  $(this).parent().find(".social-links").fadeToggle("fast");
});

/*popup actions ******************/
$(document).on("click", '[data-dismiss="true"]', function () {
  fadeOut($(this).parents(".popup-overlay")[0], 100);
});

$(document).on("click", "[data-popup]", function () {
  var modalId = $(this).attr("data-popup");
  fadeIn($("#" + modalId)[0], 50);
});

$(document).on("click", ".popup-overlay", function (e) {
  if ($(e.target).hasClass("popup-overlay")) {
    fadeOut($(this)[0], 60);
  }
});

/*search popup actions ************/

var openSearchPopup = function () {
  fadeIn($(".search-form")[0], 50);
  setTimeout(function () {
    $(".search-form input").focus();
  }, 50);
};

var closeSearchPopup = function () {
  fadeOut($(".search-form")[0], 60);
};

$(document).on("click", '[data-search="open"]', function () {
  openSearchPopup();
});

$(document).on("click", '[data-search="close"]', function () {
  closeSearchPopup();
});
