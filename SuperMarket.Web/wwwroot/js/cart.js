//cart.js

App.ready(function () {

    document.querySelectorAll(".quantity-input").forEach(input => {

        input.addEventListener("change", function () {

            let value = parseInt(this.value);

            if (isNaN(value) || value < 1) {
                value = 1;
                this.value = 1;
            }

            try {
                Toast.success("سبد خرید بروزرسانی شد");

                // TODO: AJAX update cart

            } catch (err) {
                console.error(err);
                Toast.error("خطا در بروزرسانی سبد خرید");
            }

        });

    });

});