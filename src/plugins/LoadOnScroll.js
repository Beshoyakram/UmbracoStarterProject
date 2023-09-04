export class LoadOnScrollPlugin {
    #observer;
    #isCallBackSetteled = true;
    initLoadingOnScroll( CallBackFunction  ){
        this.#observer = new IntersectionObserver( (entries) => {
            entries.forEach(async (entry) => {
                if (entry.isIntersecting  ) {
                    if( this.#isCallBackSetteled ){
                        this.#isCallBackSetteled = false;
                        await CallBackFunction();
                        this.#isCallBackSetteled = true;
                    }
                    }
                },this);
            }, );
    }
    detachLoader(){
        this.#observer.disconnect()
    }
    attachLoader(loader) {
        setTimeout(()=> {
            this.#observer.observe(loader);
        }, 100);
    }
}
  