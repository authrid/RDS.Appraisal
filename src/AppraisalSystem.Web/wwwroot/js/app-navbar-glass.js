(function () {
    var THRESHOLD_PX = 8;
    var bound = new WeakSet();

    function syncScrolled(shell) {
        shell.classList.toggle("app-navbar-shell--scrolled", window.scrollY > THRESHOLD_PX);
    }

    function bind(shell) {
        if (!shell || bound.has(shell)) {
            return;
        }

        bound.add(shell);
        var onScroll = function () {
            syncScrolled(shell);
        };

        window.addEventListener("scroll", onScroll, { passive: true });
        syncScrolled(shell);
        shell._appNavbarGlassDispose = function () {
            window.removeEventListener("scroll", onScroll);
            bound.delete(shell);
        };
    }

    function dispose(shell) {
        if (!shell || typeof shell._appNavbarGlassDispose !== "function") {
            return;
        }

        shell._appNavbarGlassDispose();
        delete shell._appNavbarGlassDispose;
    }

    window.appNavbarGlass = {
        bind: bind,
        dispose: dispose
    };
})();
