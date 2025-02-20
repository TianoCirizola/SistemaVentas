$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");

    fetch("/Dashboard/ObtenerResumen")
        .then(response => {
            $("div.container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {

                // Actualizacion de datos de las tarjetas:
                let d = responseJson.objeto;

                $("#totalVenta").text(d.totalVenta);
                $("#totalIngresos").text(d.totalIngresos);
                $("#totalProductos").text(d.totalProductos);
                $("#totalCategorias").text(d.totalCategorias);

                // Obtener textos y valores para los graficos de barras:
                let barchartLabels;
                let barchartData;

                if (d.ventasUltimaSemana.length > 0) {
                    barchartLabels = d.ventasUltimaSemana.map(item => { return item.fecha });
                    barchartData = d.ventasUltimaSemana.map(item => { return item.total });

                } else {
                    barchartLabels = ["sin resultados"];
                    barchartData = [0];
                }

                // Obtener textos y valores para los graficos de pie:
                let piecharLabels;
                let piechartData;

                if (d.productosTopUltimaSemana.length > 0) {
                    piecharLabels = d.productosTopUltimaSemana.map(item => { return item.producto });
                    piechartData = d.productosTopUltimaSemana.map(item => { return item.cantidad });

                } else {
                    piecharLabels = ["sin resultados"];
                    piechartData = [0];
                }

                // Bar Chart Example
                let controlVenta = document.getElementById("chartVentas");
                let myBarChart = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchartLabels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchartData,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });

                // Pie Chart Example
                let controlProducto = document.getElementById("chartProductos");
                let myPieChart = new Chart(controlProducto, {
                    type: 'doughnut',
                    data: {
                        labels: piecharLabels,
                        datasets: [{
                            data: piechartData,
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', "#FF785B"],
                            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', "#FF5733"],
                            hoverBorderColor: "rgba(234, 236, 244, 1)",
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        tooltips: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyFontColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            caretPadding: 10,
                        },
                        legend: {
                            display: true
                        },
                        cutoutPercentage: 80,
                    },
                });
            }
        });
});