
$(document).ready(function () {

    //KEYVALUE FORM

    const keyValuePairsContainer = $(".keyValuePairs");
    const addKeyValuePairButton = $(".addKeyValuePair");

    function updateHiddenFields(g_cont) {

        g_cont.find(".keyValuePairContainer").each(function (index) {
            let dict_name = $(this).data("dname");
            let container = $(this);

            let key = container.find(".key").val();
            let value = container.find(".value").val();
            let hidden_kv = $(this).find(".hiddenKV");
            if (value != "" && key != "") {
                hidden_kv.attr("name", dict_name + "[" + key + "]");
                hidden_kv.attr("value", value);
            }
            

        });
    }

    addKeyValuePairButton.on("click", function (event) {
        event.stopPropagation();
        const pairDiv = $("<div>");
        pairDiv.addClass("keyValuePairContainer");
        pairDiv.addClass("item-list-controlable-section");
        
        pairDiv.append('<input type="text" placeholder="Наименование" class="item-list-target-content item-list-show showMenu" readonly/>');
        pairDiv.append('<input type="hidden" class="key item-list-target-value" />');
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


    //ITEM LIST
    var item_list_current_container = null;

    $(".item-list-controlable-container").on("click", ".item-list-controlable-section .item-list-show.showMenu", function (event) {
        event.stopPropagation();
        item_list_current_container = $(this).parent();
        $(".overlayMenu").fadeIn();
    });

    $(".item-list-container .item-button-row").on("click", function (event) {
        event.stopPropagation();
        item_list_current_container.find("item-list-target-value").attr("value", $(this).find("td.item-list-id")[0].html());
        item_list_current_container.find("item-list-target-content").attr("value", $(this).find("td.item-list-name")[0].html());

        item_list_current_container.find("item-list-target-content").val().change();
        item_list_current_container.find("item-list-target-value").val().change();
        $(".overlayMenu").fadeIn();
    });





});
