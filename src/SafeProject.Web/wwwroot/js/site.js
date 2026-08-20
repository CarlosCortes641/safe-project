(() => {
  const money = (n) => "$" + Number(n).toLocaleString("en-US");

  const builder = document.querySelector("[data-safe-builder]");
  if (builder) {
    const productButtons = [...builder.querySelectorAll(".product-btn")];
    const payButtons = [...builder.querySelectorAll("[data-pay]")];
    const nameEl = builder.querySelector("[data-selected-name]");
    const cashEl = builder.querySelector("[data-cash-price]");
    const monthlyEl = builder.querySelector("[data-safe24-monthly]");
    const productId = builder.querySelector("[data-product-id]");
    const payPath = builder.querySelector("[data-pay-path]");
    const zipInput = builder.querySelector("#zip");
    const zipHidden = builder.querySelector("[data-zip]");
    const cta = builder.querySelector("[data-start-cta]");

    const payMap = {
      cash: "CashSandbox",
      sync: "SynchronySandbox",
      safe24: "Safe24Sandbox",
    };

    const syncSelection = () => {
      const selected = productButtons.find((b) => b.classList.contains("on")) || productButtons[0];
      const price = Number(selected.dataset.price || 0);
      if (nameEl) nameEl.textContent = selected.dataset.name || "";
      if (cashEl) cashEl.textContent = money(price);
      if (monthlyEl) {
        const suffix = monthlyEl.dataset.monthlySuffix || "";
        monthlyEl.textContent = `${money(((price - 100) / 24).toFixed(2))} × 24 ${suffix}`.trim();
      }
      if (productId) productId.value = selected.dataset.id || "";
      if (zipHidden && zipInput) zipHidden.value = zipInput.value || "28202";
      const pay = payButtons.find((b) => b.classList.contains("selected"));
      const payKey = pay?.dataset.pay || "cash";
      if (payPath) payPath.value = payMap[payKey] || "CashSandbox";
      if (cta) {
        const label = cta.getAttribute(`data-cta-${payKey}`) || cta.getAttribute("data-cta-cash") || "";
        cta.innerHTML = `${label} <b>→</b>`;
      }
    };

    productButtons.forEach((button) => {
      button.addEventListener("click", () => {
        productButtons.forEach((b) => b.classList.remove("on"));
        button.classList.add("on");
        syncSelection();
      });
    });

    payButtons.forEach((button) => {
      button.addEventListener("click", () => {
        payButtons.forEach((b) => b.classList.remove("selected", "hot"));
        button.classList.add("selected");
        if (button.dataset.pay === "cash") button.classList.add("hot");
        syncSelection();
      });
    });

    zipInput?.addEventListener("input", () => {
      zipInput.value = zipInput.value.replace(/\D/g, "").slice(0, 5);
      syncSelection();
    });
    syncSelection();
  }

  document.querySelectorAll(".scopegrid article").forEach((group) => {
    const buttons = [...group.querySelectorAll("button")];
    buttons.forEach((button) => {
      button.addEventListener("click", () => {
        buttons.forEach((b) => b.classList.remove("on"));
        button.classList.add("on");
      });
    });
  });

  const tons = document.querySelector("#hvac-tons");
  const need = document.querySelector("#hvac-need");
  const hvacPrice = document.querySelector("[data-hvac-price]");
  const hvacLabel = document.querySelector("[data-hvac-label]");
  const hvacProduct = document.querySelector("[data-hvac-product]");
  const hvacSubmit = document.querySelector("[data-hvac-submit]");
  const updateHvac = () => {
    if (!hvacPrice) return;
    const quoteable = need?.value === "replace";
    const option = tons?.selectedOptions?.[0];
    const amount = Number(option?.dataset.price || 0);
    const productId = tons?.value || "";
    if (!quoteable || amount === 0) {
      hvacPrice.textContent = "—";
      if (hvacLabel) hvacLabel.textContent = "Quote after inspection";
      if (hvacSubmit) hvacSubmit.disabled = true;
      return;
    }
    hvacPrice.textContent = money(amount);
    if (hvacLabel) hvacLabel.textContent = "Complete replacement";
    if (hvacProduct) hvacProduct.value = productId;
    if (hvacSubmit) hvacSubmit.disabled = false;
  };
  tons?.addEventListener("change", updateHvac);
  need?.addEventListener("change", updateHvac);
  updateHvac();

  const journey = document.querySelector("[data-journey]");
  if (journey) {
    const inspect = journey.querySelector("[data-inspect]");
    const payPath = journey.querySelector("[data-pay-path]");
    journey.querySelectorAll("[data-set-inspect]").forEach((button) => {
      button.addEventListener("click", () => {
        journey.querySelectorAll("[data-set-inspect]").forEach((b) => b.classList.remove("selected"));
        button.classList.add("selected");
        if (inspect) inspect.value = button.getAttribute("data-set-inspect") || "remote";
      });
    });
    journey.querySelectorAll("[data-set-pay]").forEach((button) => {
      button.addEventListener("click", () => {
        const value = button.getAttribute("data-set-pay") || "CashSandbox";
        journey.querySelectorAll("[data-set-pay]").forEach((b) => b.classList.remove("selected"));
        button.classList.add("selected");
        if (payPath) payPath.value = value;
        journey.querySelectorAll("[data-pay-panel]").forEach((panel) => {
          panel.hidden = panel.getAttribute("data-pay-panel") !== value;
        });
      });
    });
  }
})();
