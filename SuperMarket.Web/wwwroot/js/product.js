//product.js

App.ready(function () {

    document.querySelectorAll(".btn-add-to-cart").forEach(btn => {

        btn.addEventListener("click", async function (e) {
            e.preventDefault();

            const productId = this.dataset.productId;

            if (!productId) {
                Toast.error("شناسه محصول نامعتبر است");
                return;
            }

            try {

                // TODO: AJAX later
                // await fetch(...)

                Toast.success("محصول به سبد خرید اضافه شد");

            } catch (err) {
                console.error(err);
                Toast.error("خطا در افزودن محصول");
            }

        });

    });

});