$(document).ready(function () {
    $.getJSON("/Admin/GetProductData", function (data) {
        Highcharts.chart('container-1', {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: 'Products Price Range'
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            accessibility: {
                point: {
                    valueSuffix: '%'
                }
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: true
                }
            },
            series: [{
                name: 'Products',
                colorByPoint: true,
                data: [{
                    name: `Under 200$`,
                    y: data._productsUnder200,
                    sliced: true,
                    selected: true
                }, {
                    name: 'Between 200$ and 600$',
                    y: data._productsBetween200And600
                    }, {
                    name: 'Over 600$',
                    y: data._productsover600
                    }
                ]
            }]
        });
    })
    $.getJSON("/Admin/GetProductSaleRate", function (data) {
        Highcharts.chart('container-2', {
            title: {
                text: 'Top Sellers'
            },
            subtitle: {
                text: 'Plain'
            },
            xAxis: {
                categories: [data.Products[0], data.Products[1], data.Products[2], data.Products[3], data.Products[4], data.Products[5], data.Products[6], data.Products[7],],
                labels: {
                    autoRotationLimit: 40
                                        
                }
            },
            yAxis: {
                title: {
                    text: "Sales"
                }
            },
            series: [{
                type: 'column',
                colorByPoint: true,
                data: [data.Top5ProductSales[0], data.Top5ProductSales[1], data.Top5ProductSales[2], data.Top5ProductSales[3], data.Top5ProductSales[4], data.Top5ProductSales[5], data.Top5ProductSales[6], data.Top5ProductSales[7]],
                showInLegend: false,
            }]
        });
    })

    $.getJSON("/Admin/GetFrequentCustomers", function (data) {
        var yAxisLabels = [0, 5 , 10 , 15 , 20, 25 , 30, 35,40];
        Highcharts.chart('container-3', {
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Frequent Customers'
            },
            xAxis: {
                categories: [data.Top5CustomersInOrders[0], data.Top5CustomersInOrders[1], data.Top5CustomersInOrders[2], data.Top5CustomersInOrders[3], data.Top5CustomersInOrders[4]],
                title: {
                    text: 'Customers'
                }
            },
            yAxis: {
                allowDecimals: false,
                min: 0,
                title: {
                    text: 'Number of Orders',
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                },
                tickPositioner: function () {
                    return yAxisLabels;
                },
            },
            tooltip: {
                valueSuffix: ' Orders'
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                x: -40,
                y: 80,
                floating: true,
                borderWidth: 1,
                backgroundColor:
                    Highcharts.defaultOptions.legend.backgroundColor || '#FFFFFF',
                shadow: true
            },
            credits: {
                enabled: false
            },
            series: [{
                name: 'Orders',
                data: [data.Top5OrderId[0], data.Top5OrderId[1], data.Top5OrderId[2], data.Top5OrderId[3], data.Top5OrderId[4]]
            }]
        });
    })
})