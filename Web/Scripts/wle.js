/**
*for WarehouseLaborEfficiency
*/

function EmptyTable() {
    if (oidTable) {
        oidTable.destroy();
    }
    $('#idTable').empty();
}

function GetMyChartColors() {
    return ['#00AA00', '#CC0033', '#FF3300',
            'purple', '#99FF00', '#0033CC',
            '#CC0066', '#33CCFF', '#007777',
            '#CCFF99', '#CCFF00'
    ];
}

function GetSelect2Sel($id) {
    var sels = $id.select2('data');
    return sels ? sels[0] : null;
}


function DownloadData(dType, $frm) {
    $frm.validate();
    if ($frm.valid()) {
        var startW = GetSelect2Sel($('#idSelStartWeek'));
        var endWeek = GetSelect2Sel($('#idSelEndWeek'));
        var url = "/Query/DownloadData";
        var para = {
            dType: dType,
            bu: GetSelect2Sel($('#idSelBu')).text,
            startWeek: startW ? startW.text : null,
            endWeek: endWeek ? endWeek.text : null
        };
        url += '?';
        url += $.param(para);
        parent.location.href = url;
    }
}

