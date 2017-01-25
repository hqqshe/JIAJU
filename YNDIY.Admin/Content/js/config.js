require.config({
    baseUrl: '/Content/minjs',
    shim: {
        'pop_window': ['jquery'],
        'uploadForm': ['jquery'],
        'base': ['jquery']
    },
    paths: {
        'jquery': 'jquery-1.7.2',
        'pop_window': 'pop_window',
        'utils': 'utils',
        'domReady': 'domReady',
        'laydate': 'laydate/laydate',
        'uploadForm': 'uploadForm',
        'base': 'base',
        'pinyin': 'cn2pinyin'
    }
});