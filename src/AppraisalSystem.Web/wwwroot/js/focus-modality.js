(function () {
    var root = document.body;

    if (!root) {
        return;
    }

    function setKeyboardMode(event) {
        var key = event.key;
        if (key === "Tab" || key === "ArrowUp" || key === "ArrowDown" || key === "ArrowLeft" || key === "ArrowRight") {
            root.classList.add("user-is-tabbing");
        }
    }

    function clearKeyboardMode() {
        root.classList.remove("user-is-tabbing");
    }

    document.addEventListener("keydown", setKeyboardMode, { passive: true });
    document.addEventListener("mousedown", clearKeyboardMode, { passive: true });
    document.addEventListener("pointerdown", clearKeyboardMode, { passive: true });
    document.addEventListener("touchstart", clearKeyboardMode, { passive: true });
})();
