/**
*for WarehouseLaborEfficiency
*/

function EmptyTable() {
    if (oidTable) {
        oidTable.destroy();
    }
    $('#idTable').empty();
}

function EmptyChart(chartObj) {
    if (chartObj) {
        chartObj.clear();
    }
}

function GetMyChartColors() {
    return ['#00AA00', '#0033CC', '#99FF00',
            '#FF3300', '#33CCFF', '#007777',
            '#FF00CC', '#DD6633', 'purple',
            '#330099', '#FFEE00', '#CCCC66',
            '#9999FF'
    ];
}

function GetSelect2Sel($id) {
    var sels = $id.select2('data');
    return sels ? sels[0].id : null;
}

function GetSelect2SelMulti($id) {
    var sels = $id.select2('data');
    var sAll = '';
    $.each(sels, function (ind, val) {
        sAll += val.id;
        sAll += ',';
    });
    if (sAll.length>0) {
        sAll = sAll.slice(0, -1);
    }
    return sAll;
}

function DownloadData(dType, $frm) {
    $frm.validate();
    if ($frm.valid()) {
        var startW = GetSelect2Sel($('#idSelStartWeek'));
        var endWeek = GetSelect2Sel($('#idSelEndWeek'));
        var selYear = GetSelect2Sel($('#idSelYear'));
        var url = "/Query/DownloadData";
        var para = {
            dType: dType,
            bu: GetSelect2SelMulti($('#idSelBu')),
            startWeek: startW ? startW : null,
            endWeek: endWeek ? endWeek : null
        };
        url += '?';
        url += $.param(para);
        parent.location.href = url;
    }
}

function DownloadMonthData(selKind) {
    var url = "/Query/DownloadMonthData";
    var para = {
        'selKind': selKind
    };
    url += '?';
    url += $.param(para);
    parent.location.href = url;
}


function GridHelper(gridID) {
    var self = this;

    self.Warehouses = [
        { name: 'Mech', value: "Mech" },
        { name: 'PCBA B11', value: "PCBA B11" },
        { name: 'PCBA B13', value: "PCBA B13" },
        { name: 'PCBA B15', value: "PCBA B15" }
    ];

    self.GridID = gridID;
    self.selectedItems = [];
    self.selectItem = function (item) {
        self.selectedItems.push(item);
    };
    self.hasSelected = function () {
        return self.selectedItems && self.selectedItems.length>0;
    };
    self.unselectItem = function (item) {
        self.selectedItems = $.grep(self.selectedItems, function (i) {
            return i !== item;
        });
    };

    self.deleteSelectedItems = function(cbDelete) {
        if (!self.selectedItems.length) {
            return;
        }
        cbDelete(self.selectedItems);
        var $grid = $(self.GridID);
        $grid.jsGrid("option", "pageIndex", 1);
        $grid.jsGrid("loadData");
        self.selectedItems = [];
    };

    self.refresh = function () {
        var $grid = $(self.GridID);
        $grid.jsGrid("reset");
    };
}
