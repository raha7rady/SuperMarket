(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        // Auto-dismiss success/error alerts after a few seconds.
        var alerts = document.querySelectorAll(".alert.alert-dismissible");
        alerts.forEach(function (alertEl) {
            setTimeout(function () {
                var closeButton = alertEl.querySelector(".btn-close");
                if (closeButton) {
                    closeButton.click();
                } else {
                    alertEl.classList.remove("show");
                }
            }, 5000);
        });

        // Enable Bootstrap tooltips if any are present on the page.
        if (window.bootstrap && window.bootstrap.Tooltip) {
            var tooltipTriggers = document.querySelectorAll('[data-bs-toggle="tooltip"]');
            tooltipTriggers.forEach(function (el) {
                new window.bootstrap.Tooltip(el);
            });
        }
    });
})();
