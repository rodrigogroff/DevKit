
angular.module('app.controllers').controller('DashboardController',
    ['$scope', '$rootScope', 'Api', 
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            function init()
            {
                $scope.loading = true;

                Api.AdmOper.listPage({ op: '100' }, function (data) {

                    $scope.a = data.a; $scope.b = data.b; $scope.c = data.c;
                    $scope.d = data.d; $scope.e = data.e; $scope.f = data.f;
                    $scope.g = data.g; $scope.h = data.h;

                    // ----------------
                    // Pie Chart
                    // ----------------

		            var dataPie = [
                        { label: "Sitef", data: Number($scope.b.totalSITEF)},
                        { label: "Portal", data: Number($scope.b.totalPORTAL) },
                        { label: "Mobile", data: Number($scope.b.totalMOBILE) },
		            ];

                    $.plot(placeholder102, dataPie, {
                        series: {
                            pie: {
                                show: true
                            }
                        },
                        legend: {
                            show: false
                        }
                    });

                    var data103 = [["Sitef", data.c.totalSitef], ["Portal", data.c.totalPortal], ["Mobile", data.c.totalMobile]];

                    $.plot("#placeholder103", [data103], {
                        series: {
                            bars: {
                                show: true,
                                barWidth: 0.6,
                                align: "center"
                            }
                        },
                        xaxis: {
                            mode: "categories",
                            tickLength: 0
                        }
                    });

                    var data105 = [];

                    for (var i = 0; i < data.e.list.length; i++)
                    {
                        var item = data.e.list[i];
                        data105.push([item._empresa, item._financ]);
                    }

                    $.plot("#placeholder105", [data105], {
                        series: {
                            bars: {
                                show: true,
                                barWidth: 0.6,
                                align: "center"
                            }
                        },
                        xaxis: {
                            mode: "categories",
                            tickLength: 0
                        }
                    });

                    $scope.loading = false;
                });
            }

            $scope.refresh = function () {
                init();
            };

            $scope.forcarFech = function (e) {

                $scope.loading = true;

                Api.AdmOper.listPage({ op: '101', emp: e.st_empresa }, function (data) {

                    $scope.loading = false;
                    init();
                });
            };

            init();

        }]);
