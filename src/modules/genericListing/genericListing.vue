<template>
    <div class="init-loader" v-if="!isInitedBefore">
        <!-- Start Loader -->
        <Loader :show="!isInitedBefore" />
        <!-- End Loader -->
    </div>
    <section class="releaseSection" v-if="isInitedBefore">
        <div class="">
            <div class="row releaseSection__filter">
                  <form class="col-12 col-md-3 releaseSection__filter__item">
                      <div class="generic-group-input">
                          <input type="text"
                                 class="generic-input generic-group-input__input"
                                 v-model="generatedQuery.SearchText"
                                 :placeholder="props.SearchFor" />
                          <button class="generic-group-input__icon"
                                  @keyup.enter.prevent="searchinputValueChanged"
                                  @click.prevent="searchinputValueChanged">
                              <i class="icon-search-icon"></i>
                          </button>
                      </div>
                  </form>
                  <div class="
              datepicker__drpdown datepicker__drpdown--dateFrom
              col-12 col-md-3
              releaseSection__filter__item
            "
                       v-bind:class="{ 'datepicker--reverse-arrow': isFromCalendarOpen }">
                      <DatePicker v-model="selectedDateFrom"
                                  :max-date="dateFromDisabledDates"
                                  :locale="dateLanguage"
                                  transition="none"
                                  ref="dateFromPicker"
                                  :popover="{ placement: dateStartPosition, visibility: 'click' }"
                                  :masks="masks">
                          <template v-slot="{ inputValue, inputEvents }">
                              <div class="generic-group-input pointer">
                                  <input class="generic-input generic-group-input__input"
                                         :value="inputValue"
                                         v-on="inputEvents"
                                         readonly
                                         :placeholder="props.dateFromPlaceholder" />
                                  <span class="
                      icon-close-fill
                      generic-group-input__icon generic-group-input__icon--close
                    "
                                        v-if="selectedDateFrom != null"
                                        v-on:click="resetDate('dateFrom')"></span>
                                  <span v-else
                                        class="generic-group-input__icon icon-date pointer-none">
                                  </span>
                              </div>
                          </template>
                      </DatePicker>
                  </div>
                  <div class="
              datepicker__drpdown datepicker__drpdown--dateTo
              col-12 col-md-3
              releaseSection__filter__item
            "
                       v-bind:class="{ 'datepicker--reverse-arrow': isToCalendarOpen }">
                      <DatePicker v-model="selectedDateTo"
                                  :min-date="dateToDisabledDates"
                                  :locale="dateLanguage"
                                  transition="none"
                                  ref="dateToPicker"
                                  :popover="{ placement: dateStartPosition, visibility: 'click' }"
                                  :masks="masks">
                          <template v-slot="{ inputValue, inputEvents }">
                              <div class="generic-group-input pointer">
                                  <input class="generic-input generic-group-input__input"
                                         :value="inputValue"
                                         v-on="inputEvents"
                                         readonly
                                         :placeholder="props.dateToPlaceholder" />
                                  <span class="
                      icon-close-fill
                      generic-group-input__icon generic-group-input__icon--close
                    "
                                        v-if="selectedDateTo != null"
                                        v-on:click="resetDate('dateTo')"></span>
                                  <span v-else
                                        class="generic-group-input__icon icon-date pointer-none">
                                  </span>
                              </div>
                          </template>
                      </DatePicker>
                  </div>
                  <div class="
              multiselect__wrapper multiselect__wrapper--releaseWidth
              col-12 col-md-3
              releaseSection__filter__item releaseSection__filter__item--last
            ">
                      <VueMultiselect :placeholder="props.CategoryLabel"
                                      v-model="categoryObject"
                                      :options="detailsData.categories"
                                      @select="categoryChange"
                                      track-by="lookupName"
                                      label="lookupName"
                                      :showLabels="false"
                                      :searchable="false"
                                      :allow-empty="false">
                      </VueMultiselect>
                  </div>
                  <div class="clearFilter d-none"
                       @click="resetAllData"
                       :class="{ 'clearFilter--show': showClearAllButton }">
                      <span>{{ props.removeAllLabel }}</span><i class="icon-close-icon"></i>
                  </div>
              </div>
            <div class="row" v-if="detailsData.genericDetailsModels.length > 0">
                <div class="col-md-3"
                     v-for="(item, index) in detailsData.genericDetailsModels"
                     :key="index">
                    <div class="generic-card">
                        <div class="generic-card-content">
                            <section>
                                <a :href="item.url">
                                    <section class="generic-card-content__img">
                                        <section class="generic-card-content__img-wrapper">
                                            <img :src="item.image" :alt="item.title" />
                                        </section>
                                    </section>
                                </a>

                                <h1 class="generic-card-content__title">
                                    <a :href="item.url" :title="item.title">
                                        {{ item.title }}
                                    </a>
                                </h1>
                            </section>
                            <section class="generic-card-content__body">
                                <p class="generic-card-content__body-date">
                                    {{ item.date }}
                                    <!-- 21 يناير 2022 : 21 يناير 2022 -->
                                </p>
                            </section>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 genericListing--min-height-section"
                     v-if="!isLoading && detailsData.genericDetailsModels.length == 0">
                    <div class="genericListing__empty-message generic-empty-message">
                        <img src="../../assets/gold-no-message.svg"
                             :alt="props.noResultFound" />
                        <h3>{{ props.noResultFound }}</h3>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12"
                     v-if="isLoading"
                     :class="{
            'genericListing--min-height-section':
              detailsData.genericDetailsModels.length == 0,
          }">
                    <div class="genericListing__generic-card-contents-loader">
                        <!-- Start Loader -->
                        <Loader :show="isLoading" />
                        <!-- End Loader -->
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
                    <div id="generic-observer"></div>
                </div>
            </div>
        </div>
    </section>
</template>

<script>
    import VueMultiselect from "vue-multiselect";
    import { DatePicker } from "v-calendar";
    import axios from "axios";
    import moment from "moment";
    import Loader from "../global/components/loader/Loader.vue";
    let apiUrlMainPoint = "";

    export default {
        // eslint-disable-next-line
        components: { VueMultiselect, DatePicker, Loader },
        data() {
            return {
                date: new Date(),
                masks: {
                    input: "YYYY-MM-DD",
                    weekdays: "WWWW",
                },
                props: {},
                isInitedBefore: false,
                isLoading: false,
                dataFinished: false,
                detailsData: {},
                categoryObject: {},
                generatedQuery: {
                    ContentId: "",
                    PageNumber: 1,
                    Lang: "",
                    SearchText: "",
                    DateFrom: "",
                    DateTo: "",
                    Catogray: 0,
                    IsInit: true,
                },
                generatedQueryAsText: "",
                showClearAllButton: false,
                dateFromDisabledDates: null,
                dateToDisabledDates: null,
                dateLanguage: "en",
                selectedDateFrom: null,
                selectedDateTo: null,
                dateStartPosition: "",
                isFromCalendarOpen: false,
                isToCalendarOpen: false,
            };
        },
        mounted() {
            /* eslint-disable no-debugger */
            //debugger;
            //this.$attrs.props
            this.props = this.$attrs.props;
            apiUrlMainPoint = "umbraco/api/GenericListing/GetAll";
            this.generatedQuery.Lang = this.props.lang;
            this.generatedQuery.ContentId = this.props.pageId;
            if (this.props.lang == "ar-EG") {
                this.dateLanguage = "ar";
                this.dateStartPosition = "bottom-end";
            } else {
                this.dateLanguage = "en";
                this.dateStartPosition = "bottom-start";
                this.masks.weekdays = "WWW";
            }
            this.callApi();
        },
        methods: {
            resetDate(dateType) {
                if (dateType == "dateFrom") {
                    this.selectedDateFrom = null;
                } else {
                    this.selectedDateTo = null;
                }
            },
            toggleFromCalendar: function () {
                setTimeout(() => {
                    if (
                        document
                            .querySelector(
                                ".datepicker__drpdown--dateFrom .vc-popover-content-wrapper"
                            )
                            .classList.contains("is-interactive")
                    ) {
                        this.isFromCalendarOpen = true;
                    } else {
                        this.isFromCalendarOpen = false;
                    }
                }, 0);
            },
            toggleToCalendar: function () {
                setTimeout(() => {
                    if (
                        document
                            .querySelector(
                                ".datepicker__drpdown--dateTo .vc-popover-content-wrapper"
                            )
                            .classList.contains("is-interactive")
                    ) {
                        this.isToCalendarOpen = true;
                    } else {
                        this.isToCalendarOpen = false;
                    }
                }, 0);
            },
            fixCalendarOpenAfterApiCall: function () {
                document
                    .querySelectorAll(".datepicker__drpdown--dateFrom input")
                    .forEach((element) => {
                        element.addEventListener("click", () => {
                            this.isFromCalendarOpen = !this.isFromCalendarOpen;
                        });
                    });
                document
                    .querySelectorAll(".datepicker__drpdown--dateTo input")
                    .forEach((element) => {
                        element.addEventListener("click", () => {
                            this.isToCalendarOpen = !this.isToCalendarOpen;
                        });
                    });

                const domChangeObserverForDateFrom = new MutationObserver((mutation) => {
                    if (mutation[0].attributeName === "class") {
                        this.toggleFromCalendar();
                    }
                });

                domChangeObserverForDateFrom.observe(
                    document.querySelector(
                        ".datepicker__drpdown--dateFrom .vc-popover-content-wrapper"
                    ),
                    {
                        attributes: true,
                    }
                );

                const domChangeObserverForDateTo = new MutationObserver((mutation) => {
                    if (mutation[0].attributeName === "class") {
                        this.toggleToCalendar();
                    }
                });

                domChangeObserverForDateTo.observe(
                    document.querySelector(
                        ".datepicker__drpdown--dateTo .vc-popover-content-wrapper"
                    ),
                    {
                        attributes: true,
                    }
                );
            },
            callApi() {
                this.generateQueryForSearch();
                this.showClearButtonIfHasData();
                this.isLoading = true;

                axios
                    .post(
                        `${window.location.origin}/${apiUrlMainPoint}`,
                        this.generatedQuery
                    )
                    .then((res) => {
                        if (this.isInitedBefore == false) {
                            this.detailsData = res.data.data;
                            if (this.detailsData.genericDetailsModels == null) {
                                this.detailsData.genericDetailsModels = [];
                            }
                            this.isLoading = false;
                            this.isInitedBefore = true;
                            this.detailsData.categories.unshift({
                                lookupName: this.props.CategoryLabel,
                                id: 0,
                            });
                            this.categoryObject = {
                                lookupName: this.props.CategoryLabel,
                                id: 0,
                            };
                            setTimeout(() => {
                                this.checkObserverTrigger();
                                if (document.querySelector(".datepicker__drpdown--dateFrom") != null) {
                                    this.fixCalendarOpenAfterApiCall();
                                }
                            }, 200);
                        } else {
                            if (
                                res.data.data.genericDetailsModels != null &&
                                res.data.data.genericDetailsModels.length != 0
                            ) {
                                if (this.generatedQuery.PageNumber == 1) {
                                    this.detailsData.genericDetailsModels = res.data.data.genericDetailsModels;
                                } else {
                                    this.detailsData.genericDetailsModels = this.detailsData.genericDetailsModels.concat(
                                        res.data.data.genericDetailsModels
                                    );

                                    this.isLoading = false;
                                }
                            } else {
                                //this means that i got empty list
                                if (this.generatedQuery.PageNumber == 1) {
                                    this.detailsData.genericDetailsModels = [];
                                }
                                this.dataFinished = true;
                                this.isLoading = false;
                            }

                            this.isLoading = false;
                        }
                    })
                    .catch(() => {
                        document.querySelector("#snackbar").innerText =
                            this.props.somethingWentWrongLabel;
                        document.querySelector("#snackbar").classList.add("show");
                        setTimeout(function () {
                            document.querySelector("#snackbar").classList.remove("show");
                        }, 3000);
                        this.isLoading = false;
                    });
            },
            generateQueryForSearch() {
                this.generatedQuery.IsInit = !this.isInitedBefore;
            },
            checkObserverTrigger() {
                const options = {
                    threshold: 1.0,
                };
                const checker = document.querySelector("#generic-observer");
                const infiniteScroll = (entries) => {
                    entries.forEach((entry) => {
                        if (entry.isIntersecting) {
                            if (
                                this.detailsData != "undefined" &&
                                this.detailsData.genericDetailsModels.length != 0 &&
                                this.generatedQuery.PageNumber >= 1 &&
                                !this.dataFinished
                            ) {
                                setTimeout(() => {
                                    this.generatedQuery.PageNumber += 1;
                                    this.isLoading = true;
                                    this.callApi();
                                }, 500);
                            }
                        }
                    });
                };

                const observer = new IntersectionObserver(infiniteScroll, options);
                observer.observe(checker);
            },
            searchinputValueChanged() {
                this.dataFinished = false;
                this.generatedQuery.PageNumber = 1;
                this.detailsData.genericDetailsModels = [];
                this.callApi();
            },
            categoryChange(newVal) {
                this.dataFinished = false;
                this.generatedQuery.PageNumber = 1;
                this.detailsData.genericDetailsModels = [];

                this.generatedQuery.Catogray = newVal.id;
                this.callApi();
            },
            showClearButtonIfHasData() {
                if (
                    this.generatedQuery.SearchText == "" &&
                    this.generatedQuery.DateFrom == "" &&
                    this.generatedQuery.DateTo == "" &&
                    this.generatedQuery.Catogray == 0
                ) {
                    this.showClearAllButton = false;
                } else {
                    this.showClearAllButton = true;
                }
            },
            resetAllData() {
                this.dataFinished = false;
                this.generatedQuery.PageNumber = 1;
                this.detailsData.genericDetailsModels = [];

                this.categoryObject = {
                    name: this.props.CategoryLabel,
                    id: 0,
                };

                this.generatedQuery.SearchText = "";
                this.generatedQuery.DateFrom = "";
                this.generatedQuery.DateTo = "";
                this.generatedQuery.Catogray = 0;
                this.selectedDateFrom = null;
                this.selectedDateTo = null;
                this.callApi();
            },
        },
        watch: {
            selectedDateFrom: function () {
                this.dataFinished = false;
                this.generatedQuery.PageNumber = 1;
                this.detailsData.genericDetailsModels = [];
                if (this.selectedDateFrom != null) {
                    this.dateToDisabledDates = this.selectedDateFrom;
                } else {
                    this.dateToDisabledDates = null;
                }
                //Set Date To generatedQuery
                if (this.selectedDateFrom != null) {
                    this.generatedQuery.DateFrom = moment(this.selectedDateFrom).format(
                        "DD/MM/YYYY"
                    );
                } else {
                    this.generatedQuery.DateFrom = "";
                }

                this.callApi();
            },
            selectedDateTo: function () {
                this.dataFinished = false;
                this.generatedQuery.PageNumber = 1;
                this.detailsData.genericDetailsModels = [];

                if (this.selectedDateTo != null) {
                    this.dateFromDisabledDates = this.selectedDateTo;
                } else {
                    this.dateFromDisabledDates = null;
                }
                //Set Date To generatedQuery
                if (this.selectedDateTo != null) {
                    this.generatedQuery.DateTo = moment(this.selectedDateTo).format(
                        "DD/MM/YYYY"
                    );
                } else {
                    this.generatedQuery.DateTo = "";
                }

                this.callApi();
            },
            isToCalendarOpen() {
                if (this.isToCalendarOpen && this.isFromCalendarOpen) {
                    this.$refs.dateFromPicker.hidePopover();
                    this.isFromCalendarOpen = false;
                }
            },
            isFromCalendarOpen() {
                if (this.isFromCalendarOpen && this.isToCalendarOpen) {
                    this.$refs.dateToPicker.hidePopover();
                    this.isToCalendarOpen = false;
                }
            },
        },
    };
</script>

<style></style>
