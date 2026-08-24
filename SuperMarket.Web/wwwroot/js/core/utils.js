window.Utils = (function () {

    function debounce(func, wait) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), wait);
        };
    }

    function formatPrice(value) {
        if (value === null || value === undefined || isNaN(value)) return "0";
        return new Intl.NumberFormat("fa-IR").format(value);
    }

    function isNullOrEmpty(value) {
        return value === null || value === undefined || value.toString().trim() === "";
    }

    function clamp(value, min, max) {
        return Math.min(Math.max(value, min), max);
    }

    function parseNumber(value, fallback = 0) {
        const n = parseFloat(value);
        return isNaN(n) ? fallback : n;
    }

    return {
        debounce,
        formatPrice,
        isNullOrEmpty,
        clamp,
        parseNumber
    };

})();