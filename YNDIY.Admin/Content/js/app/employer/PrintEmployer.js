require.config({
    baseUrl: "/Content/minjs",
    paths: {
        'jquery': 'jquery-1.7.2',
        'barcode': 'jquery-barcode'
    },
    shim: {
        'barcode': 'jquery'
    }
});
require(['jquery'], function ($) {
    var _item_html = '<div class="_code_item _left _width_percent_50">\
                            <div class="_cut_line">\
                                <div class="_bar_code_desc"></div>          \
                                <div class="_bar_code_container"></div>     \
                            </div>\
                        </div>'
    var _clear_html = '<div class="_clear"></div>';
    var _null_html = '<div class="_code_item _null_item _left _width_percent_50">\
                            <div class="_cut_line"></div>\
                        </div>'
    var _code_table = '<div class="_bar_code_table _clear_panel"></div>';
    var _a4_paper = '<div class="_a4_paper"></div>';
    var _more_a4 = '<div class="_more_a4"></div>';
    var _suit_title = '<div class="_suit_title"></div>';
    require(['barcode'], function () {


        _get_bar_code();
        //获取条码
        function _get_bar_code() {
            _show_loading();
            $.ajax({
                type: "get",
                url: "/ERPemployer/DownloadEmplyerBarCodeData" + location.search,
                dataType: "json",
                success: function (response) {
                    _hide_loading();
                    if (response.code == 1) {
                        _render_bar_code(response.data);
                    } else {
                        alert(response.message);
                    }
                },
                error: function (e) {
                    _hide_loading();
                    alert("网络异常,请刷新重试！");
                }
            });
        }
        //渲染条码PANEL
        function _render_bar_code(_suit_list) {
            var _render_panel = $("._paper_render");
            _render_panel.empty();
            for (var i = 0, _len = _suit_list.length; i < _len; i++) {
                var _a4 = _render_panel.append(_render_code_list(_suit_list[i].data, _suit_list[i].title));
            }
        }
        function _hide_loading() {
            $("._loading").hide();
        }
        function _show_loading() {
            $("._loading").show();
        }
        //渲染条码细节
        function _render_code_list(_code_list, _title_text) {
            var _add_count = 0;
            var _splice_count = 0;
            var _max_count = 22;
            var _a4 = $(_a4_paper);
            var _table = $(_code_table);
            for (var j = 0; j < _code_list.length; j++) {
                _splice_count++;
                if (_code_list[j].name == "换行" && _code_list[j].code == "换行") {
                    if (j <= 19) {
                        var _null = $(_null_html);
                        if (j % 2 == 1) {
                            _table.append(_null.clone()).append(_null.clone()).append(_null);
                            _add_count += 3;
                        } else {
                            _table.append(_null.clone()).append(_null);
                            _add_count += 2;
                        }
                    } else {
                        if (j == 20) {
                            _add_count += 2;
                        } else {
                            _add_count++;
                        }
                    }
                } else {
                    _add_count++;
                    var _item = $(_item_html);
                    $("._bar_code_desc", _item).text(_code_list[j].name);
                    $("._bar_code_container", _item).barcode(_code_list[j].code, "code128", { barWidth: 1, barHeight: 40, showHRI: true });
                    _table.append(_item);
                }
                if (_add_count == _max_count) {
                    break;
                }
            }
            _table.append($(_clear_html));
            var _title = $(_suit_title).text(_title_text);
            _a4.append(_title).append(_table);
            if (_code_list.length > _splice_count) {
                var _more = $(_more_a4);
                _code_list.splice(0, _splice_count);
                _more.append(_a4);
                if (_code_list.length) {
                    _more.append(arguments.callee(_code_list, _title_text));
                }
                return _more;
            }
            return _a4;
        }
    });
})