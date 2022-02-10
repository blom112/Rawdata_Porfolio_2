define(['knockout', 'dataService', 'postman', 'jquery'], function (ko, ds, postman, $) {
    return function (params) {
        let carouselControls = () => {
            $('*[id^="cardCarousel"]').each(function () {
                if (window.matchMedia("(min-width: 768px)").matches) {
                    let carouselWidth = $("#inner_" + this.id)[0].scrollWidth;
                    let cardWidth = $(".carousel-item").width();
                    let scrollPosition = 0;
                    $("#next_" + this.id).on("click", function () {
                        if (scrollPosition < carouselWidth - cardWidth * 4) {
                            scrollPosition += cardWidth;
                            $("#inner_" + this.id).animate(
                                {scrollLeft: scrollPosition},
                                600
                            );
                        }
                    });
                    $("#prev_" + this.id).on("click", function () {
                        if (scrollPosition > 0) {
                            scrollPosition -= cardWidth;
                            $("#inner_" + this.id).animate(
                                {scrollLeft: scrollPosition},
                                600
                            );
                        }
                    });
                } else {
                    $(this).addClass("slide");
                }
            });
        };
        
        let newTitles = ko.observableArray([]);
        let favTitles = ko.observableArray([]);
        let trTitles = ko.observableArray([]);
        let randTitles = ko.observableArray([]);
        
        let getNewTitles = () => {
            console.log("getNewTitles");
            ds.getNewTitles(data => {
                console.log(data);
                newTitles(data);
            });
        }
        getNewTitles();

        let getFavTitles = () => {
            console.log("getFavTitles");
            ds.getFavTitles(localStorage.getItem('id'), data => {
                console.log(data);
                favTitles(data);
            });
        }
        getFavTitles();

        let getTrTitles = () => {
            console.log("getTrTitles");
            ds.getTrTitles(data => {
                console.log(data);
                trTitles(data);
            });
        }
        getTrTitles();

        let getRandTitles = () => {
            console.log("getRandTitles");
            ds.getRandTitles(data => {
                console.log(data);
                randTitles(data);
            });
        }
        getRandTitles();

        return {
            carouselControls,
            newTitles,
            favTitles,
            trTitles,
            randTitles,
            getNewTitles,
            getFavTitles,
            getTrTitles,
            getRandTitles
        };
    };
});