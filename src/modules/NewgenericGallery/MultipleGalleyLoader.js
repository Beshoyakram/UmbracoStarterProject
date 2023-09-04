import { createApp } from 'vue';
import genericGallery from "./genericGallery";

export function load() {
  let selector = ".generic-gallery-selector";
  // Is the custom Vue root element in the DOM?

  if (!document.querySelector(selector)) {
    return;
  }

  var elements = document.querySelectorAll(".generic-gallery-selector");
    let app;
    // Loop through the elements
    for (var i = 0; i < elements.length; i++) {
        ////eslint-disable-next-line
        //debugger;
        selector = `genericGallery-${i+1}`;
        app = createApp(genericGallery, {
            props: { ...window[selector] }
        });
        app.mount(selector);
    }
}
