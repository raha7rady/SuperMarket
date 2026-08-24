// wwwroot/js/account.js
// Lightweight, dependency-free behavior for the Account module
// (Login / Register / ChangePassword / ResetPassword).
// Relies only on window.App (core/app.js). No external libraries.

window.Account = (function () {

    // ------------------------------------------------------------
    // Password show/hide toggles.
    // Markup contract:
    //   <input id="loginPassword" type="password" />
    //   <button type="button" data-toggle-password="loginPassword" aria-label="نمایش رمز عبور">
    //       <i class="bi bi-eye"></i>
    //   </button>
    // ------------------------------------------------------------
    function initPasswordToggles() {

        const buttons = document.querySelectorAll("[data-toggle-password]");

        buttons.forEach(function (button) {

            button.addEventListener("click", function () {

                const targetId = button.getAttribute("data-toggle-password");
                const input = document.getElementById(targetId);

                if (!input) return;

                const icon = button.querySelector("i");
                const willShow = input.type === "password";

                input.type = willShow ? "text" : "password";

                if (icon) {
                    icon.classList.toggle("bi-eye");
                    icon.classList.toggle("bi-eye-slash");
                }

                button.setAttribute("aria-pressed", willShow ? "true" : "false");
                button.setAttribute(
                    "aria-label",
                    willShow ? "پنهان کردن رمز عبور" : "نمایش رمز عبور");
            });
        });
    }

    // ------------------------------------------------------------
    // Password strength meter.
    // Markup contract:
    //   <input data-password-strength
    //          data-strength-bar="strengthBar"
    //          data-strength-text="strengthText" />
    //   <div class="progress"><div id="strengthBar" class="progress-bar" role="progressbar"></div></div>
    //   <small id="strengthText"></small>
    // ------------------------------------------------------------
    function initPasswordStrength() {

        const input = document.querySelector("[data-password-strength]");

        if (!input) return;

        const bar = document.getElementById(input.getAttribute("data-strength-bar"));
        const text = document.getElementById(input.getAttribute("data-strength-text"));

        if (!bar || !text) return;

        const levels = [
            { width: "0%", color: "#dc3545", label: "" },
            { width: "20%", color: "#dc3545", label: "بسیار ضعیف" },
            { width: "40%", color: "#ffc107", label: "ضعیف" },
            { width: "60%", color: "#fd7e14", label: "متوسط" },
            { width: "80%", color: "#198754", label: "قوی" },
            { width: "100%", color: "#0d6efd", label: "بسیار قوی" }
        ];

        input.addEventListener("input", function () {

            const value = this.value;
            let score = 0;

            if (value.length >= 6) score++;
            if (value.length >= 10) score++;
            if (/[A-Z]/.test(value)) score++;
            if (/[0-9]/.test(value)) score++;
            if (/[^A-Za-z0-9]/.test(value)) score++;

            const level = levels[score] || levels[0];

            bar.style.width = level.width;
            bar.style.backgroundColor = level.color;
            bar.setAttribute("aria-valuenow", String(score * 20));
            text.textContent = level.label;
        });
    }

    function init() {
        initPasswordToggles();
        initPasswordStrength();
    }

    return { init };

})();

App.ready(Account.init);