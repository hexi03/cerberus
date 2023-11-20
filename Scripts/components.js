function generateRandomUUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

let item_list_current_container = null;

$(document).ready(function () {

    //KEYVALUE FORM

    const keyValuePairsContainer = $(".keyValuePairs");
    const addKeyValuePairButton = $(".addKeyValuePair");

    function updateHiddenFields(g_cont) {

        g_cont.parent().find(".keyValuePairContainer").each(function (index) {
            let dict_name = $(this).parent().data("dname");
            let container = $(this);

            let key = container.find(".key").val();
            let value = container.find(".value").val();
            let hidden_kv = $(this).find(".hiddenKV");
            if (value != "" && key != "") {
                hidden_kv.attr("name", dict_name + "[" + key + "]");
                hidden_kv.val(value);
            }
            

        });
    }

    addKeyValuePairButton.on("click", function (event) {
        event.stopPropagation();
        const pairDiv = $("<div>");
        pairDiv.addClass("keyValuePairContainer");
        pairDiv.addClass($(this).data("ltype") + "-list-controlable-section");
        
        pairDiv.append('<input type="text" placeholder="Наименование" class="' + $(this).data("ltype") + '-list-target-content ' + $(this).data("ltype") + '-list-show showMenu" readonly/>');
        pairDiv.append('<input type="hidden" class="key ' + $(this).data("ltype") + '-list-target-value" />');
        pairDiv.append('<input type="text" placeholder="Количество" class="value" />');
        pairDiv.append('<button class="removeKeyValuePair">Удалить</button>');
        pairDiv.append('<input type="hidden" class="hiddenKV" name="" value="" />');

        $(this).parent().append(pairDiv);

        pairDiv.find(".key, .value").on("change", function (event) {
            event.stopPropagation();
            updateHiddenFields($(this).parent());
        });

        updateHiddenFields($(this).parent());
    });

    keyValuePairsContainer.find(".keyValuePairContainer").find(".key, .value").on("change", function (event) {
        event.stopPropagation();
        updateHiddenFields($(this).parent());
    });

    keyValuePairsContainer.on("click", ".removeKeyValuePair", function (event) {
        event.stopPropagation();
        $(this).parent().remove();
        updateHiddenFields();
    });



    //OVERLAY MENU
    

    $(document).on("click", function (event) {
        event.stopPropagation();
        $(".overlayMenu").fadeOut();

    });

    $(".item-list-hide-btn").on("click", function (event) {
        event.stopPropagation();
        $(".overlayMenu").fadeOut();

    });



    $(".item-list-controlable-container").on("click", ".item-list-controlable-section .item-list-show.showMenu", function (event) {
        event.stopPropagation();
        item_list_current_container = $(this).parent();
        $(".item-list-container.overlayMenu").fadeIn();
    });

    $(".item-list-container").on("click", ".item-button-row", function (event) {
        event.stopPropagation();

        item_list_current_container.find(".item-list-target-value").val($(this).find("td.item-list-id").text());
        console.log($(this).find("td.item-list-id").text());
        item_list_current_container.find(".item-list-target-content").val($(this).find("td.item-list-name").text());
        console.log($(this).find("td.item-list-name").text());
        item_list_current_container.find(".item-list-target-content").trigger('change');
        item_list_current_container.find(".item-list-target-value").trigger('change');
        $(".overlayMenu").fadeOut();
    });

    //PRODUCTION LIST
    $(".production-list-controlable-container").on("click", ".production-list-controlable-section .production-list-show.showMenu", function (event) {
        event.stopPropagation();
        item_list_current_container = $(this).parent();
        $(".production-list-container.overlayMenu").fadeIn();
    });

    $(".production-list-container").on("click", ".production-button-row", function (event) {
        event.stopPropagation();

        item_list_current_container.find(".production-list-target-value").val($(this).find("td.production-list-id").text());
        console.log($(this).find("td.production-list-id").text());
        item_list_current_container.find(".production-list-target-content").val($(this).find("td.production-list-name").text());
        console.log($(this).find("td.production-list-name").text());
        item_list_current_container.find(".production-list-target-content").trigger('change');
        item_list_current_container.find(".production-list-target-value").trigger('change');
        $(".overlayMenu").fadeOut();
    });
    





});
