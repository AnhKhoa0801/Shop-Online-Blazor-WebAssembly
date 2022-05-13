function MakeUpdateQtyVisible(id, visible) {
    const update = document.querySelector("button[data-itemId='" + id + "']");
    if (visible) {
        update.style.display = "inline-block"
    }
    else {
        update.style.display = "none";
    }
}