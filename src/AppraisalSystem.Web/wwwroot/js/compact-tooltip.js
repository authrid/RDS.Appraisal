(() => {
    if (typeof window === "undefined") {
        return;
    }

    const supportsHover = window.matchMedia && window.matchMedia("(hover: hover)").matches;
    if (!supportsHover) {
        return;
    }

    const tooltipId = "compact-sidebar-floating-tooltip";
    let tooltip = null;
    let activeTarget = null;

    const ensureTooltip = () => {
        if (tooltip) {
            return tooltip;
        }

        tooltip = document.createElement("div");
        tooltip.id = tooltipId;
        tooltip.className = "floating-compact-tooltip";
        tooltip.setAttribute("data-show", "false");
        document.body.appendChild(tooltip);
        return tooltip;
    };

    const readTooltipText = (element) => {
        const text = element?.getAttribute("data-compact-tooltip");
        if (!text) {
            return "";
        }

        return text.trim();
    };

    const positionTooltip = (element) => {
        if (!tooltip || !element) {
            return;
        }

        const rect = element.getBoundingClientRect();
        tooltip.style.left = `${rect.right + 12}px`;
        tooltip.style.top = `${rect.top + (rect.height / 2)}px`;
    };

    const isActiveTargetValid = () => {
        if (!activeTarget) {
            return false;
        }

        if (!activeTarget.isConnected) {
            return false;
        }

        return readTooltipText(activeTarget).length > 0;
    };

    const refreshOrHideActiveTooltip = () => {
        if (!activeTarget) {
            return;
        }

        if (!isActiveTargetValid()) {
            hideTooltip();
            return;
        }

        positionTooltip(activeTarget);
    };

    const showTooltip = (element) => {
        const text = readTooltipText(element);
        if (!text) {
            hideTooltip();
            return;
        }

        const el = ensureTooltip();
        el.textContent = text;
        positionTooltip(element);
        el.setAttribute("data-show", "true");
        activeTarget = element;
    };

    const hideTooltip = () => {
        if (!tooltip) {
            activeTarget = null;
            return;
        }

        tooltip.setAttribute("data-show", "false");
        activeTarget = null;
    };

    const getTooltipTarget = (event) => {
        const source = event.target;
        if (!(source instanceof Element)) {
            return null;
        }

        return source.closest("[data-compact-tooltip]");
    };

    document.addEventListener("pointerover", (event) => {
        const target = getTooltipTarget(event);
        if (!target) {
            return;
        }

        if (target === activeTarget) {
            return;
        }

        showTooltip(target);
    });

    document.addEventListener("pointerout", (event) => {
        const target = getTooltipTarget(event);
        if (!target || target !== activeTarget) {
            return;
        }

        const next = event.relatedTarget;
        if (next instanceof Element && target.contains(next)) {
            return;
        }

        hideTooltip();
    });

    document.addEventListener("focusin", (event) => {
        const target = getTooltipTarget(event);
        if (!target) {
            return;
        }

        showTooltip(target);
    });

    document.addEventListener("focusout", (event) => {
        const target = getTooltipTarget(event);
        if (!target || target !== activeTarget) {
            return;
        }

        hideTooltip();
    });

    window.addEventListener("scroll", () => {
        refreshOrHideActiveTooltip();
    }, true);

    window.addEventListener("resize", () => {
        refreshOrHideActiveTooltip();
    });

    document.addEventListener("pointermove", () => {
        refreshOrHideActiveTooltip();
    }, { passive: true });

    document.addEventListener("click", () => {
        hideTooltip();
    });

    const observer = new MutationObserver(() => {
        refreshOrHideActiveTooltip();
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true,
        attributes: true,
        attributeFilter: ["data-compact-tooltip"]
    });
})();
