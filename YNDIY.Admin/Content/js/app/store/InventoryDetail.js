require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'domReady'], function ($, _, base, _, _) {
        base.init();
        var pid = window.location.search.split("=")[1];
        //求和
       function Sum(arr) {
            var result = 0;
            for (var i = 0; i < arr.length; i++) {
                result += arr[i];
            }
            return result;
       }
       $("._list_table input").on("blur", function () {
           var num = +$(this).val();
           var limit = +$(this).parent().prev().text();
           if (num&&limit < num) {
               $.Message({ type: "alert", content: "超过限定数量" });
               $(this).val('');
               return;
           }
       });
        //删除销售单
        $(".sub_mit").on("click",  function () {
            var _that = $(this);
            var sale_ids = [];
            var sale_count = [];
            $('.tb1 tbody tr').each(function (k,v) {
                
                var num=+$(v).children().eq(8).children().val();
                if(num){
                    sale_ids.push($(v).attr('_id'));
                    sale_count.push(num);
                }
            })
            var out_ids = [];
            var out_count = [];
            $('.tb2 tbody tr').each(function (k, v) {
                var num = +$(v).children().eq(5).children().val();
                if (num) {
                    out_ids.push($(v).attr('_id'));
                    out_count.push(num);
                }
            })
            var limit=Math.abs(+$('._list_table:eq(0) tbody tr').children().eq(9).text());
            var total = Sum(sale_count) + Sum(out_count);
            if (limit !==total ){
                $.Message({ type: "alert", content: "待处理数量为:"+limit+" 当前已处理:"+total });
                return;
            }
            var pid=$(this).attr('_id')
            $.when(base.getCodeData({ product_id: pid, sale_id_str: sale_ids.join(','), sale_count_str: sale_count.join(','), outIds: out_ids.join(','), out_count_str: out_count.join(',') }, "/ERPStore/HandleInventory")).done(function () {
                $.Message({ type: "alert", content: "保存成功！" });
                window.location.reload();
            });
        });
    });
});
