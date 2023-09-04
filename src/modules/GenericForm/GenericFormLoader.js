import { createApp } from 'vue';
import GenericForm from "./GenericForm.vue";

export function load() {
  let selector = ".generic-form-selector";
  // Is the custom Vue root element in the DOM?

  if (!document.querySelector(selector)) {
    return;
  }

  var elements = document.querySelectorAll(".generic-form-selector");
    let app;
    // Loop through the elements
    for (var i = 0; i < elements.length; i++) {
        ////eslint-disable-next-line
        //debugger;
        selector = `GenericForm-${i+1}`;
        app = createApp(GenericForm, {
            props: { ...window[selector] }
        });
        app.mount(selector);
    }
}
