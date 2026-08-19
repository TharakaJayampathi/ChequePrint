$(document).ready(function () {
    console.log('Working');
    initalLoad();
})

function initalLoad() {
    bindGrid();
}

function bindGrid() {
    var grid = $('#gridContainer').dxDataGrid({
        dataSource: "api/chequeprintreport/get-all",
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
                dataField: "chequeName",
                caption: "Cheque Name / Employee Name",
                width: 350
            },
            {
                dataField: "date",
                width: 250,
                dataType: "date",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "amount",
                caption: "Amount",
                width: 250,
                alignment: 'left',
                customizeText: function (cellInfo) {
                    const value = parseFloat(cellInfo.value);
                    if (!isNaN(value)) {
                        return value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    }
                    return "0.00";
                }
            },
            {
                dataField: "printedOn",
                dataType: "date",
                format: "yyyy-MM-dd HH:mm",
                sortOrder: "desc",
                sortIndex: 0
            }
        ],
        selection: {
            mode: "multiple",
            showCheckBoxesMode: "always"
        },
        "export": {
            enabled: true,
            fileName: "Cheque Print Report",
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
            e.component.endUpdate();
        },
        onExported: function (e) {
            e.component.beginUpdate();
            e.component.endUpdate();
        }
    }).dxDataGrid("instance");
}