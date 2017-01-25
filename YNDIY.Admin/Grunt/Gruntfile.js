module.exports = function(grunt) {

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    //concat: {
    //   options: {
    //     separator: ';',
    //   },
    //   dist: {
    //     src: ['../Content/js/plugin.js', './src/plugin2.js'],
    //     dest: './global.js',
    //   }
    // },
    uglify: {
      buildall: {//任务三：按原文件结构压缩js文件夹内所有JS文件
                files: [{
                    multistr: true,
                    expand:true,
                    cwd:'../Content/js',//js目录下
                    src:'**/*.js',//所有js文件
                    dest: '../Content/minjs'//输出到此目录下
                }]
            },
            buildapp: {//任务三：按原文件结构压缩js文件夹内所有JS文件
                files: [{
                    multistr: true,
                    expand:true,
                    cwd:'../Content/js/app',//js目录下
                    src:'**/*.js',//所有js文件
                    dest: '../Content/minjs/app'//输出到此目录下
                }]
            }
    },
    jshint: {
      all: ['../Content/js/*.js'],
    },
    watch: {
      scripts: {
        files: ['./src/plugin.js','./src/plugin2.js'],
        tasks: ['concat','jshint','uglify']
      },
      sass: {
        files: ['./scss/style.scss'],
        tasks: ['sass']
      },
      livereload: {
          options: {
              livereload: '<%= connect.options.livereload %>'
          },
          files: [
              'index.html',
              'style.css',
              'js/global.min.js'
          ]
      }
    },
    connect: {
      options: {
          port: 9000,
          open: true,
          livereload: 35729,
          // Change this to '0.0.0.0' to access the server from outside
          hostname: 'localhost'
      },
      server: {
        options: {
          port: 9001,
          base: './'
        }
      }
    },
     concat : {
            css : {

                src: ['../Content/css/template.css','../Content/css/main.css','../Content/css/popWindow.css','../Content/css/page.css','../Content/css/controls.css'],

                dest:'../Content/css/base.css'

            }

        },
        cssmin: {
        	 target: {
				    files: [{
				      expand: true,
				      cwd: '../Content/css',
				      src: ['**/*.css', '!*.min.css'],
				      dest: '../Content/mincss',
				      ext: '.min.css'
				    }]
				  }
        }
  });

  grunt.loadNpmTasks('grunt-contrib-sass');
  grunt.loadNpmTasks('grunt-contrib-concat')
  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-contrib-connect');
  grunt.loadNpmTasks('grunt-contrib-cssmin');

  grunt.registerTask('outputcss',['sass']);
  grunt.registerTask('outputcss',['sass']);
  grunt.registerTask('concatjs',['concat']);
  grunt.registerTask('compressjs',['concat','jshint','uglify']);
  grunt.registerTask('watchit',['sass','concat','jshint','uglify','connect','watch']);
  grunt.registerTask('minall', ['uglify:buildall']);//压缩全部js
  grunt.registerTask('minapp', ['uglify:buildapp']);//压缩app目录js
  grunt.registerTask('hitall', ['jshint:all']);
  grunt.registerTask('default', ['concat','cssmin']);//压缩css
};