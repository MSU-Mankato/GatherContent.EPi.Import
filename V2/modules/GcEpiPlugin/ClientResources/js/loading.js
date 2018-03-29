var config = {
    '.chosen-select': {},
    '.chosen-select-deselect': { allow_single_deselect: true },
    '.chosen-select-no-single': { disable_search_threshold: 10 },
    '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
    '.chosen-select-width': { width: "95%" }
}
$('.chosen-select').chosen();
function loadingAnimation() {
    var loading = '<div class="loading"><div class="animation-holder"><img src="/modules/GcEpiPlugin/ClientResources/img/loading-icon.png"></div></div>';
    $('body').append(loading);
}

function confirmDialog(message) {
    if (confirm(message)) {
        return loadingAnimation();
    }
    return false;
}