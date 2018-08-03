
function AjaxPost(url, data, bAsync, cbSuccess, cbError, cbBeforeSend, cbCompleteSend) {
    $.ajax({
        type: "POST",
        data: data,
        cache: false,
        url: url,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: !!bAsync,
        success: cbSuccess,
        error: cbError || function (xhr, status, errorThrown) {
                console.log("Error: " + errorThrown);
                console.log("Status: " + status);
                console.dir(xhr);
        },
        beforeSend: cbBeforeSend,
        complete: cbCompleteSend
    });
}

function AjaxPostForm(url, idForm, bAsync, cbSuccess, cbError, cbBeforeSend, cbCompleteSend) {
    var formData = new FormData($(idForm)[0]);
    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        async: !!bAsync,
        cache: false,
        contentType: false,
        processData: false,
        success: cbSuccess,
        error: cbError || function (xhr, status, errorThrown) {
            console.log("Error: " + errorThrown);
            console.log("Status: " + status);
            console.dir(xhr);
        },
        beforeSend: cbBeforeSend,
        complete: cbCompleteSend
    });
}


function reload() {
    window.location.reload(true);
}

function ShowModalDlg(id, bShow) {
    if (bShow) {
        $(id).modal('show');
    } else {
        $(id).modal('hide');
    }
}


function TTabHelper($tabLec) {
    'use strict';
    var self = this;
    self.$tab = $tabLec;

    self.AddRow = function (entry) {
        var rowNode = self.$tab
            .row.add(entry)
            .draw()
            .node();
        return rowNode;
    };
    self.GetRowByCell = function (cell) {
        return $(cell).parents('tr');
    }
    self.GetAllData = function () {
        var dt = self.$tab.rows().data().toArray();
        return dt;
    }
    self.GetDataByCell = function (cell) {
        var dt = self.$tab.row(self.GetRowByCell(cell)).data();
        return dt;
    }
    self.RemoveRow = function (cell) {
        self.$tab
            .row(self.GetRowByCell(cell))
            .remove()
            .draw();
    }
    self.Find = function (cbFilter) {
        //cbFilter: 
        //function( idx, data, node ) { return data.first_name.charAt(0) === 'A' ? true : false;}
        var res = self.$tab
                      .rows(cbFilter).data();
        return res;
    }
    self.SetClickCallback = function (fnClick) {
        self.$tab.on('click', 'tbody >tr', function () {
            var dt = self.$tab.row(this).data();
            fnClick(dt);
        });
    }
    self.SetDblClickCallback = function (fnDblClick) {
        self.$tab.on('dblclick', 'tbody >tr', function () {
            var dt = self.$tab.row(this).data();
            fnDblClick(dt);
        });
    }

}



function TSelect2Helper($sel2) {
    'use strict';
    var self = this;
    self.$sel2 = $sel2;

    self.SetSelectChangeCallback = function (cb) {
        self.$sel2.on('select2:select',
            function (e) {
                e.preventDefault();
                var data = e.params.data;
                cb(data);
            });
    }
    self.GetCurSelData = function () {
        var dt = self.$sel2.select2('data');
        return !dt ? null : dt[0];
    }
    self.ClearSelect = function (bTrigge) {
        self.$sel2.val(null).trigger('change');
    }
    self.Select = function (v) {
        self.$sel2.val(v).trigger("change");
    }
    self.RemoveAll = function () {
        self.$sel2.find('option').remove();
    }
    self.Reset = function () {
        self.RemoveAll();
        self.$sel2.off('select2:select');
        self.$sel2.data('select2').destroy();
    }
}

//object value to []
function MakeValueArr(obj) {
    //return Object.values(obj);
    var res = [];
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) {
            res.push(obj[i]);
        }
    }
    return res;
}


