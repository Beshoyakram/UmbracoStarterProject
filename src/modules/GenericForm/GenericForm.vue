<template>
  <div class="container">
    <div class="generic-form">
      <div class="row">
        <div class="offset-md-1 col-md-10">
          <div class="generic-form__header">
            <div class="generic-form__header__icon">
              <img src="./letter.svg" />
            </div>
            <h1 class="generic-form__header__main-title">
              {{ $attrs.props.contactUsTitle }}
            </h1>
          </div>
        </div>
      </div>
      <div class="row">
        <div class="offset-md-1 col-md-10">
          <div class="generic-form__content__container">
            <form
              class="generic-form__content"
              @submit.prevent="vaildateForm"
              v-show="!isFormSubmitted"
            >
              <p class="generic-form__content__title">
                {{ $attrs.props.EnterYourData }}
              </p>

              <input
                type="text"
                class="generic-form__generic-field"
                v-model="v$.formSumbittedObject.Name.$model"
                @input="v$.formSumbittedObject.Name.$touch"
                :placeholder="$attrs.props.FullName"
              />
              <span
                v-show="
                  v$.formSumbittedObject.Name.regex.$invalid ||
                  v$.formSumbittedObject.Name.noSpecialChar.$invalid
                "
                class="generic-form__error-msg"
              >
                {{ $attrs.props.FullNameNotCorrect }}
              </span>
              <span
                v-show="v$.formSumbittedObject.Name.required.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.FullNameRequired }}
              </span>
              <span
                v-show="v$.formSumbittedObject.Name.maxlength.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.FullNameMaxLength }}
              </span>
              <input
                type="text"
                class="generic-form__generic-field"
                v-model="v$.formSumbittedObject.Email.$model"
                @input="v$.formSumbittedObject.Email.$touch"
                :placeholder="$attrs.props.EmailAddress"
              />

              <span
                v-show="
                  v$.formSumbittedObject.Email.Email.$invalid &&
                  v$.formSumbittedObject.Email.$model
                "
                class="generic-form__error-msg"
              >
                {{ $attrs.props.EmailNotCorrect }}
              </span>

              <span
                v-show="v$.formSumbittedObject.Email.requiredIf.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.EmailRequiredIfPhonenotExist }}
              </span>
              <input
                type="text"
                class="generic-form__generic-field --phone"
                v-model="v$.formSumbittedObject.Phone.$model"
                @input="v$.formSumbittedObject.Phone.$touch"
                :placeholder="$attrs.props.Phone"
              />

              <span
                v-show="
                  v$.formSumbittedObject.Phone.maxlength.$invalid ||
                  v$.formSumbittedObject.Phone.minlength.$invalid
                "
                class="generic-form__error-msg"
              >
                {{ $attrs.props.PhoneMaxAndMinLength }}
              </span>

              <span
                v-show="v$.formSumbittedObject.Phone.validateNum.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.NationalNumbers }}
              </span>
              <span
                v-show="v$.formSumbittedObject.Phone.requiredIf.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.PhoneRequiredIfEmailnotExist }}
              </span>

              <input
                type="text"
                class="generic-form__generic-field"
                v-model="v$.formSumbittedObject.MsgTitle.$model"
                @input="v$.formSumbittedObject.MsgTitle.$touch"
                :placeholder="$attrs.props.MessageTitle"
              />

              <span
                v-show="v$.formSumbittedObject.MsgTitle.required.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.MessageTitleRequired }}
              </span>
              <span
                v-show="v$.formSumbittedObject.MsgTitle.noSpecialChar.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.MessageTitleNotCorrect }}
              </span>
              <textarea
                class="generic-form__generic-field --text-area"
                v-model="v$.formSumbittedObject.MsgContent.$model"
                @input="v$.formSumbittedObject.MsgContent.$touch"
                :placeholder="$attrs.props.Message"
              ></textarea>

              <span
                v-show="v$.formSumbittedObject.MsgContent.required.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.MessageRequired }}
              </span>
              <span
                v-show="
                  v$.formSumbittedObject.MsgContent.noSpecialChar.$invalid
                "
                class="generic-form__error-msg"
              >
                {{ $attrs.props.MessageNotCorrect }}
              </span>
              <span
                v-show="v$.formSumbittedObject.MsgContent.maxlength.$invalid"
                class="generic-form__error-msg"
              >
                {{ $attrs.props.MessageMaxLength }}
              </span>

              <!-- :hl="`${culture}`" -->
              <VueRecaptcha
                class="recaptcha"
                :sitekey="$attrs.props.RecaptchaSiteKey"
                size="normal"
                theme="light"
                @verify="recaptchaVerified"
                @expired="recaptchaExpired"
                @fail="recaptchaFailed"
                ref="vueRecaptcha"
              >
              </VueRecaptcha>

              <span
                v-show="isRecaptchaNotValid"
                class="generic-form__error-msg --recaptcha"
              >
                {{ $attrs.props.RecaptchaRequired }}
              </span>
              <button class="generic-form__submit-btn" type="submit">
                {{ $attrs.props.SubmitFormBtn }}
              </button>
            </form>
            <div class="init-loader" v-show="showLoader">
              <loader :show="showLoader"></loader>
            </div>

            <div class="generic-form__success-msg" v-show="isFormSubmitted">
              <img
                src="../../assets/success-msg.svg"
                class="generic-form__success-msg__icon"
              />
              <p class="generic-form__success-msg__text">
                {{ $attrs.props.SuccessMessage }}
              </p>
            </div>
            <generic-toast
              :title="toasterTitle"
              :toasterText="toasterText"
              :show="showTosat"
              @closed="show = false"
              :failedTrial="failedTrial"
            >
            </generic-toast>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable */
import { useVuelidate } from "@vuelidate/core";
import { VueRecaptcha } from "vue-recaptcha";
import GenericToast from "../GenericToast/GenericToast.vue";
import axios from "axios";

import {
  maxLength,
  helpers,
  minLength,
  email,
  requiredIf,
  required,
} from "@vuelidate/validators";
import Loader from "../global/components/loader/Loader.vue";

const noSpecialChar = (value) => {
  let regex = /([~`!@#$%^&*()_\-+={}\[\]|\/\:\;<>'",\\?\|.])+/gm;
  let isSpecialChar = regex.test(value);
  return !isSpecialChar;
};
const validateNum = (phone) => {
  let regex = /^(?:\+?(:|[0-9]| ){8,15}){0,1}$/;
  let isValidPhone = regex.test(phone);
  return isValidPhone;
};
export default {
  components: {
    VueRecaptcha,
    GenericToast,
    Loader,
  },
  data() {
    return {
      v$: useVuelidate(),
      formSumbittedObject: {
        Name: "",
        Email: "",
        Phone: "",
        MsgTitle: "",
        MsgContent: "",
        ReCaptchaToken: "",
        contactUsId: this.$attrs.props.genericBasedPageID,
      },
      isRecaptchaNotValid: null,
      isFormSubmitted: false,
      toasterTitle: this.$attrs.props.SomethingIsWrongTitle,
      toasterText: this.$attrs.props.SomethingIsWrongBody,
      showTosat: false,
      failedTrial: true,
      showLoader: false,
    };
  },
  validations() {
    return {
      formSumbittedObject: {
        Name: {
          regex: helpers.regex(/^\D+$/),
          noSpecialChar,
          $lazy: true,
          required,
          maxlength: helpers.withMessage("message here", maxLength(256)),
        },
        Email: {
          Email: helpers.withMessage("message here", email),
          requiredIf: requiredIf(function () {
            return this.formSumbittedObject.Phone == "";
          }),
          $lazy: true,
        },
        Phone: {
          maxlength: helpers.withMessage("message here", maxLength(15)),
          minlength: helpers.withMessage("message here", minLength(8)),
          validateNum,
          $lazy: true,
          requiredIf: requiredIf(function () {
            return this.formSumbittedObject.Email == "";
          }),
        },
        MsgTitle: {
          required,
          noSpecialChar,
          $lazy: true,
        },
        MsgContent: {
          required,
          noSpecialChar,
          $lazy: true,
          maxlength: helpers.withMessage("message here", maxLength(1000)),
        },
      },
    };
  },

	methods: {
        vaildateForm() {
            debugger;
			this.v$.$validate();
			if (this.formSumbittedObject.ReCaptchaToken == "") {
				this.isRecaptchaNotValid = true;
			} else {
				const isFormValid = this.v$.$errors.length == 0 && this.formSumbittedObject.ReCaptchaToken != "";
				if (isFormValid) {
					this.isRecaptchaNotValid = false;
                    this.showLoader = true;

                    /*axios.post(`${window.location.origin}/umbraco/api/ContactUs/SubmitForm`, this.formSumbittedObject)*/
                    var bodyFormData = new FormData();
                    bodyFormData.append('Email', this.formSumbittedObject.Email);
                    bodyFormData.append('MsgContent', this.formSumbittedObject.MsgContent);
                    bodyFormData.append('Name', this.formSumbittedObject.Name);
                    bodyFormData.append('Phone', this.formSumbittedObject.Phone);
                    bodyFormData.append('ReCaptchaToken', this.formSumbittedObject.ReCaptchaToken);
                    bodyFormData.append('contactUsId', this.formSumbittedObject.contactUsId);
                    bodyFormData.append('uploadFile', this.formSumbittedObject.uploadFile);

                    axios({
                        method: "post",
                        url: `${window.location.origin}/umbraco/api/ContactUs/SubmitForm`,
                        data: bodyFormData,
                        headers: { "Content-Type": "multipart/form-data" },
                    })
                    .then((res) => {
                        debugger;
						if (res.status == 200) {
							this.isFormSubmitted = true;
							this.showLoader = false;
							this.formSumbittedObject.ReCaptchaToken = ""

						} else {
							this.failedTrial = true;
							this.showTosat = true;
							this.showLoader = false;

							setTimeout(()=> {
							this.showTosat = false;

							},3000)

						}
					}).catch(() => {
						this.showTosat = true;
						this.failedTrial = true;
						this.showLoader = false;
						setTimeout(()=> {
							this.showTosat = false;

							},3000)
					})
				}

			}

		},
		recaptchaVerified(response) {
			this.formSumbittedObject.ReCaptchaToken = response;
			this.isRecaptchaNotValid = false;
		},
		recaptchaExpired() {
			this.$refs.vueRecaptcha.reset();
			this.formSumbittedObject.ReCaptchaToken = "";
			this.isRecaptchaNotValid = true;
		},
		recaptchaFailed() {
			this.$refs.vueRecaptcha.reset();
			this.formSumbittedObject.ReCaptchaToken = "";
			this.isRecaptchaNotValid = true;
		},
	},
}

</script>