<template>
	<div class="position-fixed bottom-0 start-50 pb-5 translate-middle-x toaster-container" style="z-index: 11">
		<!-- data-bs-autohide="false" -->
		<div ref="toaster" id="liveToast"  class="toast hide alert-danger  " role="alert" aria-live="assertive"
			aria-atomic="true">
			<img src="./error-icon.svg" v-if="failedTrial" class="error-icon"/>
			<div class="tosater-content">
				<div class="toast-header">
					<strong>{{title}}</strong>
					<button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close">
						<img src="./close-toaster.svg"/>
					</button>
				</div>
				<div class="toast-body">
					{{toasterText}}
				</div>
			</div>

		</div>
	</div>
</template>

<script>
import { Toast } from "bootstrap"
export default {
	props: ['title', 'toasterText', 'show', 'failedTrial'],
	emits: ['shown', 'closed'],
	data() {
		return {
			toast: undefined,
		}
	},
	mounted() {
		this.toast = new Toast(this.$refs.toaster);
		this.$refs.toaster.addEventListener("shown.bs.toast", () => this.$emit('shown'))
		this.$refs.toaster.addEventListener("hidden.bs.toast", () => this.$emit('closed'))
	},
	watch: {
		show: {
			handler() {
				if (this.show) {
					this.$nextTick(() => {
						this.toast.show()
					})

				} else {
					this.$nextTick(() => {
						this.toast.hide()
					})
				}
			},
			immediate: true,
		}
	}
}
</script>