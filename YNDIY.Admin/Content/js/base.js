define(['jquery'], function ($) {
    return {
        //初始公共组件事件
        init: function () {
            //载入图标gif 关闭
            window._hide_loading = function () {
                $("._loading").hide();
            },
            //载入图标gif 打开
            window._show_loading = function () {
                $("._loading").show();
            }
            //输入框回车事件 2017-1-5添加输入框清空按钮显示事件 2017-1-13 添加查询按钮切换图片
            $('.search_input').on("keyup", function (e) {
                if (e.keyCode == 13) {
                    window._load_items && _load_items(1);
                }
                $(this).val() && $(this).next().children('.cross').show() || $(this).next().children('.cross').hide();
            }).focus(function () {
                $(this).next().addClass('discover');
            }).blur(function () {
                $(this).next().removeClass('discover');
            });
            //输入框清空按钮
            $('.cross').on("click", function (e) {
                e.stopPropagation();
                $(this).parent().prev().val('');
                $(this).hide();
            });
            //查询事件 搜索按钮
            $(".search_btn").on("click", function () {
                window._load_items && _load_items(1);
            });
            //查询事件 下拉框
            $(".select_load").on("change", function () {
                window._load_items && _load_items(1);
            });
            //弹出层 关闭按钮左上
            $("._pop_close").on("click", function () {
                $(this).parents("._pop_window").eq(0).fadeOut();
                $("body").css({ "overflow-y": "auto" });
            });
            // //弹出层 关闭按钮 自定
            $("._pop_window").on("click", ".cancle", function () {
                $(this).parents("._pop_window").eq(0).fadeOut();
            });

            //表格操作菜单 begin
            var special_btn;//需要归位的弹出按钮
            $("._more_btn ._show_more").live("click", function () {
                var _this = $(this);
                $("._relative_zindex_1").removeClass("_relative_zindex_1");
                _this.parents("._relative").addClass("_relative_zindex_1");
                $("._more_btn ._more_btns").hide();
                special_btn && special_btn.css("top", 0);
                var panl = _this.next();
                panl.show();
                //var distance = $("._content_render_panel").height() - (panl.offset().top + panl.height());
                //if (distance < 0) {
                //    panl.css({ top: distance + "px" }, 300);
                //    special_btn = panl;
                //}
                return false;
            });
            $("html,body").on("click", function () {
                $("._more_btn ._more_btns").hide();
                special_btn && special_btn.css("top", 0);
                $('._diy_select').hide();
            });
            //表格操作菜单 end
            //banner菜单切换
            $('._block_menu').on('click', function () {
                var parent = $(this).parent();
                if (!parent.hasClass('open'))
                    parent.addClass('open').siblings().removeClass('open');
            });
            //select_opacity下拉框
            $("._search_filter ._opacity_select").live("change", function () {
                $(this).prev().find("._select_text").text($(this.options[this.selectedIndex]).text());
            });
            //banner菜单初始化
            var _request_url = document.location.pathname.toLowerCase();
            $(".child_wrap a").each(function (k, o) {
                if (typeof $(o).attr("links") != "undefined") {
                    var _link = $(o).attr("links").toLowerCase().split(";");
                    for (var i = 0; i < _link.length; i++) {
                        if (_request_url == _link[i]) {
                            $(o).addClass("cur_menu").parents('.menu_item').eq(0).addClass('open');
                            return false;
                        }
                    }
                }
            });
            //返回上一级页面
            $('.back').on('click', function () {
                window.history.back();
            });
            //表格复选框全选
            $(".click_tb,._ajax_container").on("click", ".all_cbo", function () {
                $(this).parents("thead").next().find("input[type=checkbox]").click();
            });
            //表格复选框单选
            $(".click_tb,._ajax_container").on("click", "tbody input[type=checkbox]", function () {
                var tr = $(this).parents("tr").eq(0);
                tr.toggleClass("plus");
                var tbody = $(this).parents('tbody');
                if (tbody.find('.plus').length === tbody.find('tr input[type="checkbox"]').length) {
                    tbody.prev().find('.all_cbo').attr("checked", 'true');
                } else {
                    tbody.prev().find('.all_cbo').removeAttr("checked");
                }
            });
            //前一天、后一天
            $("._date_page").on("click", function () {
                var _date_render = $("._search_date");
                var _date = _date_render.text().replace(/\s/g, "").replace(/-/g, "/");
                var _option_day;
                if ($(this).hasClass("_my_prev")) {
                    //前一天
                    _option_day = new Date(new Date(_date).valueOf() - 24 * 60 * 60 * 1000);
                } else {
                    //后一天
                    _option_day = new Date(new Date(_date).valueOf() + 24 * 60 * 60 * 1000);
                }
                _date = _option_day.getFullYear() + "-" + ((_option_day.getMonth() + 1) > 9 ? _option_day.getMonth() + 1 : "0" + (_option_day.getMonth() + 1)) + "-" + (_option_day.getDate() > 9 ? _option_day.getDate() : "0" + _option_day.getDate());
                _date_render.text(_date);
                window._load_items && _load_items(1);
            });
            //diy_select 打开
            $(".select_diy").on("click", '._select_prev', function () {
                $('._diy_select').hide();
                var _diy_select=$(this).next();
                if (_diy_select) {
                    $(this).next().show();
                    $("input", _diy_select).val("");
                }
                return false;
            });
            $(".select_diy").on("click", '._diy_select', function () {
                return false;
            });
            //选择 diy_select 项
            $(".select_diy").on("click", '._diy_select a', function () {
                var _me = $(this);
                var _text = _me.text();
                var _id = _me.attr("_id");
                var parent = _me.parents("._diy_select").eq(0);
                parent.prev().find("._select_text").text(_text).attr("_id", _id);
                parent.hide();
            });
            //diy_select关键字筛选
            $(".select_diy").on("keyup", '._diy_select input', function () {
                var _me = $(this);
                var _diy_select = _me.parents('._diy_select').eq(0);
                var _val = _me.val().replace(/[^0-9a-zA-Z-\+_]/, "");
                _me.val(_val);
                if (_val == "") {
                    $("li", _diy_select).show();
                } else {
                    eval("var _regex = /" + _val.toLowerCase() + "/");
                    $("li", _diy_select).each(function () {
                        var _o = $(this);
                        var _py = _o.attr("_py");
                        if (_regex.test(_py)) {
                            _o.show();
                        } else {
                            _o.hide();
                        }
                    });
                }
            });

        },
        repainWindow: function () {
            var width = $('._ajax_container ._list_table').width(), w1 = $('._search_banner ').width();
            width = width+30 > w1 ? width+54 : w1+54;
            $('body,html').width(width);
            $('._search_banner ').width();
        },
        /**
        * 加载数据 ajax
        *_data 参数, _url 链接, _type 类型get/post空为get
        * 返回html结构 
        */
        loadHtmlData: function (_data, _url, _type) {
            if (!_type) {
                _type = 'get';
            }
            _data._rdm = Math.random();
            var defer = $.Deferred();
            _show_loading();
            $.ajax({
                type: _type,
                url: _url,
                data: _data,
                dataType: "html",
                success: function (response) {
                    _hide_loading();
                    defer.resolve(response);
                },
                error: function (e) {
                    _hide_loading();
                    $.Message({ type: "alert", content: "网络异常！" });
                }
            });
            return defer.promise();
        },
        /**
        * 返回code的ajax 
        *_data 参数, _url 链接,_type 类型get/post 不传get
        * 返回response.code == 1 成功
        */
        getCodeData: function (_data, _url, _type) {
            if (!_type) {
                _type = 'get';
            }
            _data._rdm = Math.random();
            var defer = $.Deferred();
            _show_loading();
            $.ajax({
                type: _type,
                url: _url,
                data: _data,
                dataType: "json",
                success: function (response) {
                    _hide_loading();
                    if (response.code == 1) {
                        defer.resolve();
                    } else {
                        defer.fail();
                        $.Message({ type: "alert", content: response.message });
                    }
                },
                error: function (e) {
                    _hide_loading();
                    $.Message({ type: "alert", content: "网络异常！" });
                }
            });
            return defer.promise();
        },
        /**
         * 一般ajax 
         *_data 参数, _url 链接,_type 类型get/post空为get
         * 返回response.data 对象
         */
        getData: function (_data, _url, _type) {
            if (!_type) {
                _type = 'get';
            }
            _data._rdm = Math.random();
            var defer = $.Deferred();
            _show_loading();
            $.ajax({
                type: _type,
                url: _url,
                data: _data,
                dataType: "json",
                success: function (response) {
                    _hide_loading();
                    if (response.code == 1) {
                        defer.resolve(response.data);
                    } else {
                        $.Message({ type: "alert", content: response.message });
                    }
                },
                error: function (e) {
                    _hide_loading();
                    $.Message({ type: "alert", content: "网络异常！" });
                }
            });
            return defer.promise();
        },
        /**
      * 一般ajax 
      *_data 参数, _url 链接,_type 类型get/post空为get
      * 返回response 对象
      */
        getResponsData: function (_data, _url, _type) {
            if (!_type) {
                _type = 'get';
            }
            _data._rdm = Math.random();
            var defer = $.Deferred();
            _show_loading();
            $.ajax({
                type: _type,
                url: _url,
                data: _data,
                dataType: "json",
                success: function (response) {
                    _hide_loading();
                    if (response.code == 1) {
                        defer.resolve(response);
                    } else {
                        $.Message({ type: "alert", content: response.message });
                    }
                },
                error: function (e) {
                    _hide_loading();
                    $.Message({ type: "alert", content: "网络异常！" });
                }
            });
            return defer.promise();
        }
    }
});