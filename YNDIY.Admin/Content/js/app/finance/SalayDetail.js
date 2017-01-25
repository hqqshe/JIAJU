require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'domReady'], function ($, _, base, utils, _) {
        base.init();
        var td_item = "  <tr>                                                                                                      \
                                    <td>统计时间</td>                                                                      \
                                    <td>统计时间</td>                                                                      \
                                    <td>订单号</td>                                                                           \
                                    <td>生产编号</td>                                                                      \
                                    <td>型号</td>                                                                                \
                                    <td>规格</td>                                                                                \
                                    <td width='150px'><div class='remark'></div></td>                                                                                \
                                    <td>制作要求</td>                                                                      \
                                    <td>工序号</td>                                                                           \
                                    <td>工序名称</td>                                                                      \
                                    <td>完成数量</td>                                                                      \
                                    <td>工序单价（元）</td>                                                       \
                                    <td>计件总计（元）</td>                                                       \
                                </tr>";
        var search = window.location.search;
        //数字日期转换 参数date like"/Date(1478593289073)/"
        function getRightDate(date) {
            return new Date(+date.substring(6, date.length - 2)).toLocaleString()
        }
        // initData();
        //获取详情数据
        function initData() {
            _show_loading();
            $.ajax({
                type: "get",
                url: "/ERPProcedure/EmployeeSalayStatisticsDetail",
                data: { "scanStartDate": "@ViewBag.scanStartDate", "scanEndDate": "@ViewBag.scanEndDate", "userId": "@ViewBag.userId", type: " @ViewBag.type" },
                success: function (response) {
                    _hide_loading()
                    if (response.code == 1) {
                        showDetailList(response.data);
                        $('.total_pay_1').text(response.total.toFixed(2));
                    } else {
                        alert(response.message);
                    }
                },
                error: function () {
                    _hide_loading();
                    alert("网络异常,请刷新重试");
                }
            });
        }
        //展示详情
        function showDetailList(data) {
            var con = $('.list_con tbody');
            con.empty();
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < data[i].processList.length; j++) {
                    var tr = $(td_item).clone();
                    tr.children().eq(0).text(data[i].processList[j].start_produce_time);//后台接口添加数据 /ERPProcedure/EmployeeSalayStatisticsDetail
                    tr.children().eq(1).text(getRightDate(data[i].processList[j].scan_data));//后台接口添加数据
                    tr.children().eq(2).text(data[i].order_id);
                    tr.children().eq(3).text(data[i].produce_id);
                    tr.children().eq(4).text(data[i].model_name);
                    tr.children().eq(5).text(data[i].format);
                    tr.children().eq(6).children().text(data[i].remarks).attr("title", data[i].remarks);
                    tr.children().eq(7).text($.trim(data[i].special_remarks));
                    tr.children().eq(8).text(data[i].processList[j].step_id);
                    tr.children().eq(9).text(data[i].processList[j].step_name);
                    tr.children().eq(10).text(data[i].processList[j].number + "/" + (data[i].processList[j].unit == 0 ? "套" : data[i].processList[j].unit == 1 ? "件" : data[i].processList[j].unit == 2 ? "把" : data[i].processList[j].unit == 3 ? "个" : "张"));
                    tr.children().eq(11).text(data[i].processList[j].price);
                    tr.children().eq(12).text((data[i].processList[j].price * data[i].processList[j].number).toFixed(2));
                    con.append(tr);
                }
            }
        }
        $('._btns').on('click', 'a', function () {
            if ($(this).index() === 0) {
                window.open("/ERPFinance/SalayPrint?" + search.substring(1))
            }
        });
    });
});