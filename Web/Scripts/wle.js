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

