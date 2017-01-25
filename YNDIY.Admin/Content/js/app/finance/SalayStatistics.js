require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'pop_window', 'laydate', 'domReady'], function ($, base) {
        base.init();
        require(['laydate'], function () {
            var start_date = {
                elem: '#start_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                choose: function (datas) {
                    end_date.min = datas; //开始日选好后，重置结束日的最小日期
                    end_date.start = datas; //将结束日的初始值设定为开始日
                }
            };
            var end_date = {
                elem: '#end_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                isclear: false,
                issure: false,
                choose: function (datas) {
                    start_date.max = datas; //结束日选好后，重置开始日的最大日期
                }
            };
            laydate(start_date);
            laydate(end_date);
            $("#start_date").val(laydate.now(-30));
            $("#end_date").val(laydate.now());
        });
        initPartmentList();
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ type: $('._type').val(), ScanStartDate: $("#start_date").text(), ScanEndDate: $("#end_date").text(), DepartmentId: $('.department').val(), searchKey: $(".search_input").val(), pageIndex: _page_index }, "/ERPFinance/GetSalayStatistics")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //初始小组
        function initPartmentList() {
            $.when(base.getData({}, ' /ERPPartment/GetPartmentsIgnoreStatus')).done(function (res) {
                fillSelect(res, $('.department'), false);
            });
        }
        //$('.department').on("change", function () {
        //    initEmployer($(this).val());
        //});
        //初始小组工号
        function initEmployer(departMentId) {
            _show_loading();
            $.ajax({
                type: "get",
                url: "/ERPemployer/GetEmployerListIgnoreStatusJson?departMentId=" + departMentId,
                success: function (response) {
                    _hide_loading()
                    fillSelectEm(response.data, $('.employer'), true);
                },
                error: function () {
                    _hide_loading();
                    //alert("网络异常,请刷新重试");
                    $.Message({ type: "alert", content: "网络异常,请刷新重试！" });
                }
            });
        }
        //填充筛选条件生产小组par_list :data对象 work_group:下拉框select all:默认全部
        function fillSelect(par_list, work_group, all) {
            var op = $('<option>全部</option>');
            work_group.empty();
            if (all) {
                work_group.append(op.clone()).prev().find("._select_text").text("全部");
            }
            var option = op.clone();
            option.val("-1");
            work_group.append(option);
            for (var i = 0; i < par_list.length; i++) {
                var option = op.clone();
                var namePrefix = "";
                if (par_list[i].status == 1) {
                    namePrefix = "【已删除】";
                }
                option.text(namePrefix + par_list[i].department_name);
                option.val(par_list[i].id);
                work_group.append(option);
            }
            work_group.trigger("change");
        }
        //填充筛选条件工号
        function fillSelectEm(par_list, work_group, all) {
            work_group.empty();
            if (all) {
                work_group.append(op.clone().attr("value", "")).prev().find("._select_text").text("全部");
            }
            for (var i = 0; i < par_list.length; i++) {
                var option = op.clone();

                var employeePrefix = "";
                if (par_list[i].status == 1) {
                    employeePrefix = "【已删除】";
                }
                option.text($.trim(employeePrefix + par_list[i].no + "-" + par_list[i].name));
                option.val(par_list[i].id);
                work_group.append(option);
            }
            work_group.trigger("change");
        }
        //获取详情数据
        $('._ajax_container').on('click', '.view_detail', function () {
            var userid = $(this).parents("tr").attr("_id");
            var scan_time = $('.scan_time').text();
            var type = $('.chose_btns .blue').index();
            scan_time = scan_time.split("至");
            var start = $.trim(scan_time[0]);
            var end = $.trim(scan_time[1]);
            window.open("/ERPFinance/SalayDetail?scanStartDate=" + start + "&scanEndDate=" + end + "&userId=" + userid + "&type=" + type)
        });
        function validate() {
            var start_date = $(".start_date").val();
            if (start_date == "" || start_date == 0) {
                $.Message({ type: "alert", content: "请选择起始扫码日期！" });
                return true;
            }
            var end_date = $(".end_date").val();
            if (end_date == "" || end_date == 0) {
                $.Message({ type: "alert", content: "请选择结束扫码日期！" });
                return true;
            }
            if (end_date < start_date) {
                $.Message({ type: "alert", content: "结束扫码日期必须大于开始扫码日期！" });
                return true;
            }
        }
    });
});