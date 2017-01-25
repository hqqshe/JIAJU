require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'pop_window', 'domReady'], function ($, base) {
        base.init();
        var brand_item = ' <tr class="select">                                                                       \
                                                <td></td>                                                                                      \
                                                <td><a href="javascript:;" class="dele">×</a></td>   \
                                            </tr>';
        var cid = 0;
        initSelet();
        initCustomer()
        //初始客户信息
        function initCustomer() {
            var _search = window.location.search;
            if (/id=\d+/.test(_search)) {
                var _arr = _search.match(/id=(\d+)/);
                cid = _arr[1];
                GetCustomerInfo();
            }
        }
        //从后台获取客户信息
        function GetCustomerInfo() {
            $.when(base.getData({ id: cid }, "/ERPCustomer/GetCustomerInfoJson")).done(function (res) {
                RenderCustomerInfo(res);
            });
        }
        //初始客户信息
        function RenderCustomerInfo(_data) {
            $("._input_panel input[type='text']").each(function (i, o) {
                i == 0 ? $(o).val(_data.shop_name) :
                i == 1 ? $(o).val(_data.fandian) :
                i == 2 ? $(o).val(_data.link_man) :
                i == 3 ? $(o).val(_data.jiesuan) :
                i == 4 ? $(o).val(_data.phone) :
                i == 5 ? $(o).val(_data.piaoju) :
                i == 6 ? $(o).val(_data.address_detail) :
                i == 7 ? $(o).val(_data.order_requires) :
                i == 8 ? $(o).val(_data.fahuo) :
                i == 9 ? $(o).val(_data.huoyun) :
                i == 10 ? $(o).val(_data.credit_limit) :
                i == 11 ? $(o).val(_data.huoyun_phone) :
                i == 12 ? $(o).val(_data.tags) :
                i == 13 ? $(o).val(_data.remarks) :
                i == 14 ? $(o).val(_data.fahuo) :
                i == 15 ? $(o).val(_data.remarks) : null;
            });
            $("._input_panel input[name=check]").eq(_data.is_examine).click();
            $("._input_panel input[name=lock_store]").eq(_data.is_lockStore).click();
            if (_data.brands_id) {
                eval("brand_arr=" + _data.brands_id)
                for (var i = 0; i < brand_arr.length; i++) {
                    var item = $(brand_item);
                    var select = $('#brand_select').clone();
                    var text = select.removeAttr("id").show().find('._opacity_select').val(brand_arr[i]).find('option[value=' + brand_arr[i] + ']').attr("selected", true).text();
                    select.find('._select_text').text(text);
                    item.children().first().append(select);
                    $('.brand_tab').append(item);
                }
            }
        }
        //初始所有品牌
        function initSelet() {
            $.when(base.getData({}, "/ERPBrand/GetAllBranList")).done(function (res) {
                fillSelect(res);
            });
        }
        //初始所有品牌 填充
        function fillSelect(data) {
            var con = $('#brand_select ._opacity_select')
            con.empty();
            for (var i = 0; i < data.length; i++) {
                var item = $('<option value="1"></option>');
                item.attr("value", data[i].id).text(data[i].name);
                con.append(item);
            }
            con.val(data[0].id).prev().find('._select_text').text(data[0].name);
        }
        //添加品牌
        $('.add_brand').on('click', function () {
            var item = $(brand_item);
            var select = $('#brand_select').clone();
            select.removeAttr("id").show();
            item.children().first().append(select);
            $('.brand_tab').append(item);
        })
        //删除品牌
        $('.brand_tab').on('click', '.dele', function () {
            var txt = $(this).parent().prev().find("._select_text").text();
            var that = $(this);
            $.Message({
                type: "confirm", content: "确定删除 " + txt + " 品牌？", okFun: function () {
                    that.parent().parent().remove();
                }
            });
        })
        //获取输入客户信息
        function getInfoItems() {
            var _items_data = {};
            $("._input_panel input[type='text']").each(function (i, o) {
                i == 0 ? _items_data.customer_name = $(o).val() :
                 i == 1 ? _items_data.fandian = $(o).val() :
                i == 2 ? _items_data.link_name = $(o).val() :
                i == 3 ? _items_data.jiesuan = $(o).val() :
                i == 4 ? _items_data.link_phone = $(o).val() :
                i == 5 ? _items_data.piaoju = $(o).val() :
                i == 6 ? _items_data.link_address = $(o).val() :
                i == 7 ? _items_data.order_requires = $(o).val() :
                i == 8 ? _items_data.fahuo = $(o).val() :
                 i == 9 ? _items_data.huoyun = $(o).val() :
                i == 10 ? _items_data.credit_limit = $(o).val() :
                 i == 11 ? _items_data.huoyun_phone = $(o).val() :
                 i == 12 ? _items_data.tags = $(o).val() :
                 i == 13 ? _items_data.remarks = $(o).val() : null;
            });
            _items_data.is_examine = $("._input_panel input[name=check]:checked").val();
            _items_data.is_lockStore = $("._input_panel input[name=lock_store]:checked").val();
            _items_data.brand_id = [];
            $('.brand_tab ._opacity_select').each(function (k, v) {
                _items_data.brand_id.push(parseInt($(v).val()));
            });
            return _items_data;
        }
        //检验必填写项
        function vali(_data) {
            if (_data.customer_name == "") {
                alert("请输入客户名称！");
                return true;
            }
            if (_data.link_name == "") {
                alert("请输入联系人姓名！");
                return true;
            }
            if (_data.link_phone == "") {
                alert("请输入联系人电话！");
                return true;
            }
            if (_data.link_address == "") {
                alert("请输入联系地址！");
                return true;
            }
            if (_data.credit_limit == "") {
                alert("请输入信用额度！");
                return true;
            }
            for (var i = 0, l = _data.brand_id.length; i < l; i++) {
                for (var j = i + 1; j < l; j++)
                    if (_data.brand_id[i] === _data.brand_id[j]) {
                        alert("一个品牌只能选择一次，不能重复！");
                        return true;
                    }
            }
            return false;
        }
        //提交
        $(".sub_mit").on("click", function () {
            var _data = getInfoItems();
            if (vali(_data)) {
                return;
            }
            _data.id = cid;
            _data.brand_id = _data.brand_id.join(',');
            $.when(base.getCodeData(_data, "/ERPCustomer/saveCustomer", "post")).done(function () {
                window.location.href = "/ERPCustomer/CustomerList";
            });
        });
    });
});