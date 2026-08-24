window.Toast = (function () {

    function getContainer() {
        let container = document.getElementById("toast-container");

        if (!container) {
            container = document.createElement("div");
            container.id = "toast-container";
            container.className = "toast-container position-fixed top-0 start-0 p-3";
            container.style.zIndex = "1080";
            document.body.appendChild(container);
        }

        return container;
    }

    function escapeHtml(text) {
        const div = document.createElement("div");
        div.textContent = text;
        return div.innerHTML;
    }

    function show(message, type = "success", timeout = 4000) {

        if (!message) return;

        const container = getContainer();

        const toast = document.createElement("div");

        toast.className = `toast align-items-center text-bg-${type} border-0 show mb-2`;
        toast.setAttribute("role", "alert");

        const safeMessage = escapeHtml(message);

        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    ${safeMessage}
                </div>
                <button type="button"
                        class="btn-close btn-close-white me-2 m-auto"
                        aria-label="Close"></button>
            </div>
        `;

        const closeBtn = toast.querySelector("button");

        const remove = () => {
            toast.classList.add("fade");
            setTimeout(() => toast.remove(), 200);
        };

        closeBtn.addEventListener("click", remove);

        container.appendChild(toast);

        setTimeout(remove, timeout);
    }

    return {
        show,
        success: (m) => show(m, "success"),
        error: (m) => show(m, "danger"),
        warning: (m) => show(m, "warning"),
        info: (m) => show(m, "info")
    };

})();