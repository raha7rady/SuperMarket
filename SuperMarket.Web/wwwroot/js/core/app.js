//wwwroot/js/core/app.js

window.App = (function () {

    const App = {};

    App.ready = function (fn) {
        if (document.readyState !== "loading") {
            fn();
        } else {
            document.addEventListener("DOMContentLoaded", fn);
        }
    };

    return App;

})();