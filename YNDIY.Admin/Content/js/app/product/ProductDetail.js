require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base',  'pop_window','domReady'], function ($, base) {
        base.init();
        var _title_btn_html = '<a href="javascript:void(0);" class="_btn right _left "></a>';
        var _step_html = '<div class="_left _width_percent_25 _step_block">                                          \
                        <div class="_step_out_line">                                                             \
                            <div class="_clear_panel _step_item_title">                                          \
                                <a href="javascript:void(0);" class="_left">                                     \
                                    <span><span class="_s_id"></span>-<span class="_s_name">开料</span></span>   \
                                </a>                                                                             \
                                <a href="javascript:void(0);" class="_right">                                    \
                                </a>                                                                             \
                            </div>                                                                               \
                            <div class="_item_list_line">                                                        \
                                <div class="_item_list_title _left">工序价格(元):</div>                          \
                                <div class="_item_list_contnt">                                                  \
                                    <span class="_s_price">15.5</span>                                           \
                                </div>                                                                           \
                            </div>                                                                               \
                            <div class="_item_list_line">                                                        \
                                <div class="_item_list_title _left">步骤截止日期:</div>                          \
                                <div class="_item_list_contnt">                                                  \
                                    <span class="_s_scan_date">2016-09-13</span>                                 \
                                </div>                                                                           \
                            </div>                                                                               \
                            <div class="_item_list_line">                                                        \
                                <div class="_item_list_title _left">扫码时间:</div>                              \
                                <div class="_item_list_contnt">                                                  \
                                    <span class="_s_scan_date">2016-09-13 14:48:26</span>                        \
                                </div>                                                                           \
                            </div>                                                                               \
                            <div class="_item_list_line">                                                        \
                                <div class="_item_list_title _left">进度:</div>                                  \
                                <div class="_item_list_contnt">                                                  \
                                    <div class="_progress_bar _clear_panel">                                     \
                                        <div class="_complate_bar" style="width:20%;"></div>                     \
                                        <div class="_error_bar" style="width:30%;"></div>                        \
                                    </div>                                                                       \
                                </div>                                                                           \
                            </div>                                                                               \
                            <div class="_item_list_line">                                                        \
                                <div class="_item_list_contnt">                                                  \
                                    <div class="_progress_desc">                                                 \
                                        <span class="_complete_color _color_span"></span>                        \
                                        <span class="_color_tip _complate_text">20%</span>                       \
                                        <span class="_error_color _color_span"></span>                           \
                                        <span class="_color_tip _error_text">30%</span>                          \
                                    </div>                                                                       \
                                </div>                                                                           \
                            </div>                                                                               \
                        </div>                                                                                   \
                    </div>';
        var _process_list = null;
        var planId = window.location.search.split("=")[1];
        GetProcessInfo();
        //获取工段工序信息
        function GetProcessInfo() {
            $.when(base.getData({ id: planId }, "/ERPProduct/GetProductProcessJson")).done(function (res) {
                RenderProcess(AnalysisProcess(res));;
            });
        }
        //工序分组
        function AnalysisProcess(_data) {
            var list = [];
            for (var i = 0; i < _data.length; i++) {
                var _item = {};
                _item.id = _data[i].stage_id;
                _item.name = _data[i].stage;
                _item.item = [];
                if (list.length > 0) {
                    var _is_exits = false;
                    for (var j = 0; j < list.length; j++) {
                        if (list[j].id == _data[i].stage_id) {
                            _is_exits = true;
                            break;
                        }
                    }
                    if (!_is_exits) {
                        list.push(_item);
                    }
                } else {
                    list.push(_item);
                }
            }
            for (var i = 0; i < list.length; i++) {
                for (var j = 0; j < _data.length; j++) {
                    if (list[i].id == _data[j].stage_id) {
                        list[i].item.push(_data[j]);
                    }
                }
            }
            return list;
        }
    //渲染步骤
    function RenderStep(index) {
        var process_container = $("._process_container");
        process_container.empty();
        var data = {};
        for (var i = 0; i < _process_list.length; i++) {
            if (_process_list[i].id == index) {
                data = _process_list[i];
                break;
            }
        }
        var count = parseInt($('.head_info tbody tr').eq(0).children().eq(8).text());
        var step_data = data.item;
        for (var i = 0; i < step_data.length; i++) {
            var _step = $(_step_html);
            _step.find("._s_id").text(step_data[i].id);
            _step.find("._s_name").text(step_data[i].name);
            _step.find("._s_price").text(step_data[i].price);
            _step.find("._s_scan_date:eq(0)").text($('.head_info tbody tr').eq(0).children().eq(2).text());
            _step.find("._s_scan_date:eq(1)").text(step_data[i].scan_date);
            _step.find("._complate_bar").css({ "width": (step_data[i].complate / count) * 100 + "%" });
            _step.find("._error_bar").css({ "width": (step_data[i].error / count) * 100 + "%" });
            _step.find("._complate_text").text((step_data[i].complate / count) * 100 + "%");
            _step.find("._error_text").text((step_data[i].error / count) * 100 + "%");
            process_container.append(_step);
        }
    }
    //渲染工段信息
    function RenderProcess(list) {
        var title_container = $(".Procedures");
        _process_list = list;
        for (var i = 0; i < list.length; i++) {
            var _title_btn = $(_title_btn_html).text(list[i].name).attr("_id", list[i].id);
            _title_btn.bind("click", function () {
                var _this = $(this);
                _this.addClass("blue").siblings().removeClass("blue");
                RenderStep(_this.attr("_id"));
            });
            title_container.append(_title_btn);
        }
        $(".Procedures a:eq(0)").click();
    }
    });
});