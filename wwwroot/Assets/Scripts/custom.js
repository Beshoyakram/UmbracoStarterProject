

//==================== Adding Class to body if iPhone or iPad ====================//
if (navigator.userAgent.includes("iPad")) {
	document.querySelector("body").classList.add("body--ipad", "body--macOs");
}
if (navigator.userAgent.includes("iPhone")) {
	document.querySelector("body").classList.add("body--iphone", "body--macOs");
}
//==================== Adding Class to body if iPhone or iPad ====================//


window.addEventListener("load", () => {
	/***** aos *******/
	AOS.init({
		disable: false,
		once: true,
		startEvent: "DOMContentLoaded",
		initClassName: "aos-init",
		animatedClassName: "aos-animate",
		useClassNames: false,
		disableMutationObserver: false,
		debounceDelay: 50,
		throttleDelay: 99,
		offset: 120,
		delay: 0,
		// duration: 300,
		easing: "ease-in-out",
		mirror: false,
		anchorPlacement: "top-bottom",
	});
});


//===========================================  Subscription function  Start =========================================//

const subscription = (() => {
	let subscription;
	const getSubscriptionInstance = () => {
		if (!subscription) subscription = createNewInstance();
		return subscription;
	};
	const createNewInstance = () => {
		return Object.freeze({
			view: {
				form: document.querySelector(".subscription"),
				subscriptionBtn: document.querySelector(
					".subscription button"
				),
				inputContainer: document.querySelector(
					".subscription .subscription__input-container"
				),
				inputElement: document.querySelector(".subscription input"),
				errorLabel: document.querySelector(".subscription label"),
				newsLoaderElement() {
					return subscription
						? subscription.viewUtils.stringToHtmlElement(`
                    <div class="loader loader-btn" >
                        <div class="lds-spinner">
                            <div></div>
                            <div></div>
                            <div></div>
                            <div></div>
                            <div></div>
                            <div></div>
                            <div></div>
                            <div></div>
                        </div>
                    </div>`)
						: "";
				},
				loaderInstance: undefined,
				get loaderElement() {
					if (!this.loaderInstance)
						this.loaderInstance = this.newsLoaderElement();
					return this.loaderInstance;
				},
				showBtnLoader() {

					this.appendLoader();
					this.loaderElement.style.display = "flex";
					this.subscriptionBtn.disabled = true;
				},
				appendLoader() {

					if (!subscription.data.isLoaderAppended) {
						this.subscriptionBtn.appendChild(this.loaderElement);
						subscription.data.isLoaderAppended = true;
					}
				},
				hideBtnLoader() {

					this.appendLoader();
					this.loaderElement.style.display = "none";
					this.subscriptionBtn.style.pointerEvents = "auto";
					this.subscriptionBtn.disabled = false;
				},
				renderValidView() {

					!this.errorLabel.classList.contains("show") ||
						this.errorLabel.classList.remove("show");

					!this.inputElement.classList.contains("is-invalid") ||
						this.inputElement.classList.remove("is-invalid");

					this.inputElement.classList.add("is-valid");
					this.inputContainer.classList.add("is-valid");
				},
				renderInvalidView() {

					!this.inputElement.classList.contains("is-valid") ||
						this.inputElement.classList.remove("is-valid");

					!this.inputContainer.classList.contains("is-valid") ||
						this.inputContainer.classList.remove("is-valid");

					this.inputElement.classList.add("is-invalid");
					this.errorLabel.classList.add("show");
				},
				disableFormElements() {
					this.inputElement.disabled = true;
					this.subscriptionBtn.disabled = true;
				},
				enableFormElements() {
					this.inputElement.disabled = false;
					this.subscriptionBtn.disabled = false;
				},
				showSnackBar(message) {
					document.querySelector("#snackbar").innerText = message;
					document.querySelector("#snackbar").classList.add("show");
					setTimeout(function () {
						document.querySelector("#snackbar").classList.remove("show");
					}, 3000);
				},
			},
			viewUtils: {
				stringToHtmlElement(html) {
					var template = document.createElement("template");
					html = html.trim(); // Never return a text node of whitespace as the result
					template.innerHTML = html;
					return template.content.firstChild;
				},
			},
			data: {
				defaultErrorMessage: window.subscriptionFailMessage,
				defaultSuccessMessage: window.subscriptionSuccessMessage,
				email: "",
				isEmailDirty: false,
				isEmailValid: false,
				isLoaderAppended: false,
			},

			listeners: {
				onClickSubscribe(e) {
					e.preventDefault();
					subscription.listeners.onEmailChange();
					if (
						subscription.data.isEmailValid &&
						subscription.data.isEmailDirty
					) {
						subscription.view.showBtnLoader();
						subscription.view.disableFormElements();
						subscription.services
							.submitForm()
							.then((res) => {
								subscription.view.hideBtnLoader();
								if (res.savedSuccessfuly) {
									subscription.view.inputElement.value =
										subscription.data.defaultSuccessMessage;
									//subscription.view.showSnackBar(subscription.data.defaultSuccessMessage)
									subscription.view.disableFormElements();
									subscription.view.renderValidView();
								} else throw new Error("something went wrong");
							})
							.catch(() => {
								subscription.view.hideBtnLoader();
								subscription.view.showSnackBar(
									subscription.data.defaultErrorMessage
								);
								subscription.view.enableFormElements();
							});
					}
				},
				onEmailChange() {
					let email = subscription.view.inputElement.value;
					subscription.data.email = email;
					if (subscription.methods.checkIsFormValid(subscription.data.email)) {
						subscription.data.isEmailDirty = true;
						subscription.data.isEmailValid = true;
					} else {
						subscription.data.isEmailValid = false;
					}
					this.onEmailValidated();
				},
				onEmailValidated() {
					if (subscription.data.isEmailValid) {
						subscription.view.renderValidView();
					} else subscription.view.renderInvalidView();
				},
			},
			methods: {
				checkIsFormValid(email) {
					let emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/gim;
					return emailRegex.test(email);
				},
			},
			services: {
				async submitForm() {
					let res = fetch("/umbraco/api/NewsLetterSubscription/SubmitSubscription", {
						method: "POST",
						body: JSON.stringify({
							EmailOfSubscription: subscription.data.email,
							NewsLetterSubscriptionId: subscriptionPageID,
						}),
						headers: {
							"Content-type": "application/json; charset=UTF-8"
						}
					});
					//let res = await post(
					//	`/umbraco/api/NewsLetterSubscription/SubmitSubscription?EmailOfSubscription=${subscription.data.email}&NewsLetterSubscriptionId=${subscriptionPageID}`
					//);
					return await res.json();
				},
			},
		});
	};

	return getSubscriptionInstance;
})();

if (document.querySelector(".subscription") != null) {
	subscription().view.form.onsubmit = subscription().listeners.onClickSubscribe.bind(
		subscription().listeners
	);
	subscription().view.inputElement.onchange = subscription().listeners.onEmailChange.bind(
		subscription().listeners
	);
}
//===========================================  Subscription function End =========================================//