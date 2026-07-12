export function listenStorage(key, dotnetRef) {
  window.addEventListener("storage", (e) => {
    if (e.key === key) {
      dotnetRef.invokeMethodAsync("OnStorageChanged");
    }
  });
}