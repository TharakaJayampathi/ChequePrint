$(document).ready(function () {
    console.log('Working');
    initalLoad();
})

function initalLoad() {
    bindGrid();
}

function bindGrid() {
    var canUpdate = $('#inputCanUpdate').val();
    var canDownloadFile = $('#inputCanFileDownload').val();
    console.log(canDownloadFile);
    var isExportEnabled = false;
    if (canDownloadFile) {
        isExportEnabled = true;
    }
    var grid = $('#gridContainer').dxDataGrid({
        dataSource: "api/AgreementType/GetAll",
        onContentReady: function () {
            $(".dx-header-row").addClass("grid-header");
        },
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        sorting: {
            mode: "multiple"
        },
        columns: [
            {
                dataField: "action",
                caption: "Actions",
                width: 120,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    if (canUpdate) {
                        if (options.data.isActive == true) {
                            $("<div class='ml-2'/>").dxButton({
                                icon: "fa fa-edit fa-2xs",
                                type: "normal",
                                hint: "Edit",
                                elementAttr: {
                                    class: "btn btn-2xs",
                                },
                                text: "",
                                onClick: function (e) {
                                    edit(options);
                                }
                            }).appendTo(container);
                        }
                        $("<div class='ml-2'/>").dxButton({
                            icon: options.data.isActive == true ? "fa fa-times fa-2xs" : "fa fa-check fa-2xs",
                            type: "normal",
                            hint: options.data.isActive == true ? "Deactivate" : "Activate",
                            elementAttr: {
                                class: "btn btn-2xs"
                            },
                            text: "",
                            onClick: function (e) {
                                toggleActivate(options);
                            }
                        }).appendTo(container);
                    }
                }
            },
            {
                dataField: "code",
                caption: "Agreement Code",
                width: 150
            },
            {
                dataField: "agreementTypeName",
                caption: "Agreement Type",
                width: 250
            },
            {
                dataField: "createdUser",
                caption: "Created By",
                width: 200
            },
            {
                dataField: "createdOn",
                width: 150,
                dataType: "date",
                format: "yyyy-MM-dd HH:mm",
                sortOrder: "desc",
                sortIndex: 0
            },
            {
                dataField: "updatedUser",
                caption: "Updated By",
                width: 200
            },
            {
                dataField: "updatedOn",
                width: 150,
                dataType: "date",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "isActive",
            },
            {
                caption: "Is Active",
                dataField: "isActiveStr",
                visible: false,
            }
        ],
        selection: {
            mode: "multiple",
            showCheckBoxesMode: "always"
        },
        "export": {
            enabled: isExportEnabled,
            fileName: "Agreement Types",
            allowExportSelectedData: true
        },
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [10, 20, 50],
            showInfo: true
        },

        searchPanel: {
            visible: false
        },
        filterRow: {
            visible: true
        },
        showBorders: true,
        showRowLines: true,
        wordWrapEnabled: true,
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.addClass("fw-bold");
            }
        },
        onExporting: function (e) {
            console.log('hit 1');
            e.component.beginUpdate();
            e.component.columnOption("isActiveStr", "visible", true);
            e.component.columnOption("isActive", "visible", false);
            e.component.columnOption("action", "visible", false);
            e.component.endUpdate();
        },
        onExported: function (e) {
            console.log('hit 2');
            e.component.beginUpdate();
            e.component.columnOption("isActiveStr", "visible", false);
            e.component.columnOption("isActive", "visible", true);
            e.component.columnOption("action", "visible", true);
            e.component.endUpdate();
        }
    }).dxDataGrid("instance");
    console.log('done');
}