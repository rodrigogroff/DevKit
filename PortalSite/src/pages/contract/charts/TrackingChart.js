import React, { PureComponent } from 'react';
import $ from 'jquery';

class TrackingChart extends PureComponent {

  getTrackingChartData() { // eslint-disable-line

    const lineFlow = [];
    const points = [];
    const pointsWhite = [];

    // http://www.flotcharts.org/flot/examples/series-types/index.html

    switch (this.props.typeChart.toString()) {
      default:
      case "1":
        lineFlow.push([1, 0.3]);
        lineFlow.push([2, 0.32]);
        lineFlow.push([3, 0.34]);
        lineFlow.push([4, 0.35]);
        lineFlow.push([5, 0.35]);
        lineFlow.push([6, 0.35]);
        lineFlow.push([7, 0.34]);
        lineFlow.push([8, 0.32]);
        lineFlow.push([9, 0.28]);
        lineFlow.push([10, 0.24]);
        lineFlow.push([11, 0.22]);
        lineFlow.push([12, 0.21]);
        lineFlow.push([13, 0.2]);

        lineFlow.push([40, 0.2]);

        pointsWhite.push([5, 0.35]);
        points.push([12.5, 0.21]);
        points.push([20.5, 0.2]);
        points.push([28.5, 0.2]);
        points.push([36, 0.2]);
        break;

      case "2":
        lineFlow.push([1, 0.20]);
        lineFlow.push([2, 0.20]);
        lineFlow.push([3, 0.21]);
        lineFlow.push([4, 0.21]);
        lineFlow.push([5, 0.22]);
        lineFlow.push([6, 0.22]);
        lineFlow.push([7, 0.23]);
        lineFlow.push([8, 0.24]);
        lineFlow.push([9, 0.26]);
        lineFlow.push([10, 0.28]);
        lineFlow.push([11, 0.32]);
        lineFlow.push([12, 0.35]);
        lineFlow.push([13, 0.35]);
        lineFlow.push([14, 0.32]);
        lineFlow.push([15, 0.28]);
        lineFlow.push([16, 0.26]);
        lineFlow.push([17, 0.24]);
        lineFlow.push([18, 0.23]);
        lineFlow.push([19, 0.22]);
        lineFlow.push([20, 0.22]);
        lineFlow.push([21, 0.21]);
        lineFlow.push([22, 0.21]);
        lineFlow.push([23, 0.2]);
        lineFlow.push([40, 0.2]);

        points.push([5, 0.22]);
        pointsWhite.push([12.5, 0.35]);
        points.push([20.5, 0.22]);
        points.push([28.5, 0.2]);
        points.push([36, 0.2]);

        break;

      case "3":
        lineFlow.push([1, 0.2]);

        lineFlow.push([9, 0.2]);
        lineFlow.push([10, 0.2]);
        lineFlow.push([11, 0.21]);
        lineFlow.push([12, 0.21]);
        lineFlow.push([13, 0.22]);
        lineFlow.push([14, 0.22]);
        lineFlow.push([15, 0.23]);
        lineFlow.push([16, 0.24]);
        lineFlow.push([17, 0.26]);
        lineFlow.push([18, 0.28]);
        lineFlow.push([19, 0.32]);
        lineFlow.push([20, 0.35]);
        lineFlow.push([21, 0.35]);
        lineFlow.push([22, 0.32]);
        lineFlow.push([23, 0.28]);
        lineFlow.push([24, 0.26]);
        lineFlow.push([25, 0.24]);
        lineFlow.push([26, 0.23]);
        lineFlow.push([27, 0.22]);
        lineFlow.push([28, 0.22]);
        lineFlow.push([29, 0.21]);
        lineFlow.push([30, 0.21]);
        lineFlow.push([31, 0.2]);
        lineFlow.push([40, 0.2]);

        points.push([5, 0.2]);
        points.push([12.5, 0.21]);
        pointsWhite.push([20.5, 0.35]);
        points.push([28.5, 0.22]);
        points.push([36, 0.2]);

        break;

      case "4":
        lineFlow.push([1, 0.2]);

        lineFlow.push([18, 0.2]);
        lineFlow.push([19, 0.21]);
        lineFlow.push([20, 0.21]);
        lineFlow.push([21, 0.22]);
        lineFlow.push([22, 0.22]);
        lineFlow.push([23, 0.23]);
        lineFlow.push([24, 0.24]);
        lineFlow.push([25, 0.26]);
        lineFlow.push([26, 0.28]);
        lineFlow.push([27, 0.32]);
        lineFlow.push([28, 0.35]);
        lineFlow.push([29, 0.35]);
        lineFlow.push([30, 0.32]);
        lineFlow.push([31, 0.28]);
        lineFlow.push([32, 0.26]);
        lineFlow.push([33, 0.24]);
        lineFlow.push([34, 0.23]);
        lineFlow.push([35, 0.22]);
        lineFlow.push([36, 0.22]);
        lineFlow.push([37, 0.21]);
        lineFlow.push([38, 0.21]);
        lineFlow.push([39, 0.2]);
        lineFlow.push([40, 0.2]);

        points.push([5, 0.2]);
        points.push([12.5, 0.2]);
        points.push([20.5, 0.21]);
        pointsWhite.push([28.5, 0.35]);
        points.push([36, 0.22]);

        break;

      case "5":
        lineFlow.push([1, 0.2]);
        lineFlow.push([28, 0.2]);
        lineFlow.push([29, 0.21]);
        lineFlow.push([30, 0.22]);
        lineFlow.push([31, 0.24]);
        lineFlow.push([32, 0.28]);
        lineFlow.push([33, 0.32]);
        lineFlow.push([34, 0.34]);
        lineFlow.push([35, 0.35]);
        lineFlow.push([36, 0.35]);
        lineFlow.push([37, 0.35]);
        lineFlow.push([38, 0.34]);
        lineFlow.push([39, 0.32]);
        lineFlow.push([40, 0.30]);

        points.push([5, 0.2]);
        points.push([12.5, 0.2]);
        points.push([20.5, 0.2]);
        points.push([28.5, 0.2]);
        pointsWhite.push([36, 0.35]);

        break;
    }

    return [
      { data: lineFlow, dataPoints: points, dataPointsWhite: pointsWhite },
    ];
  }

  componentDidMount() {
    this.chart = this.createChart(this.getTrackingChartData());
    this.legend = this.$chartContainer.find('.legendLabel');
    this.initEventListeners();
  }

  createChart(data) {
    return $.plotAnimator(this.$chartContainer, [{
      label: data[0].label,
      data: data[0].data,
      lines: {
        fill: true,
      },
    },
    {
      data: data[0].dataPoints,
      points: { show: true }
    },
    {
      data: data[0].dataPointsWhite,
      points: { show: true }
    }], {
        legend: {
          show: false,
        },
        colors: ['#81B3E6', '#AAAAAA', '#19D433'],
        grid: {
          show: false,
          clickable: false,
          borderWidth: 0,
          borderColor: '#ffffff',
          margin: 0,
          minBorderMargin: 0,
          labelMargin: 40,
          mouseActiveRadius: 6,
          color: '#ffffff',
        },
        xaxis: {
          tickLength: 0,
          tickDecimals: 0,
          min: 1,
          max: 40,
          font: {
            lineHeight: 13,
            weight: 'bold',
            color: '#c1ccd3',
          },
        },
        yaxis: {
          min: 0,
          max: 0.55,
          tickDecimals: 0,
          font: {
            lineHeight: 13,
            weight: 'bold',
            color: '#c1ccd3',
          },
        },
      });
  }

  initEventListeners() {
    const self = this;

    this.$chartContainer.bind('plothover', (event, pos) => {
      if (!self.updateLegendTimeout) {
        self.updateLegendTimeout = setTimeout(self.updateLegendContent.bind(self, event, pos), 50);
      }
    });
  }

  updateLegendContent(event, pos) {
    this.updateLegendTimeout = null;

    const axes = this.chart.getAxes();
    if (pos.x < axes.xaxis.min || pos.x > axes.xaxis.max ||
      pos.y < axes.yaxis.min || pos.y > axes.yaxis.max) {
      return;
    }

    const dataset = this.chart.getData();
    for (let i = 0; i < dataset.length; i += 1) {
      const series = dataset[i];
      let point1;
      let point2;

      // Find the nearest points, x-wise
      for (let j = 0; j < series.data.length; j += 1) {
        if (series.data[j][0] > pos.x) {
          point1 = series.data[j - 1];
          point2 = series.data[j];
          break;
        }
      }

      let y;
      // Now Interpolate
      if (point1 == null && point2) {
        y = point2[1];
      } else if (point2 == null && point1) {
        y = point1[1];
      } else {
        /* eslint-disable */
        if (point1[1] !== undefined)
          y = point1[1] + (point2[1] - point1[1]) * (pos.x - point1[0]) / (point2[0] - point1[0]);
        /* eslint-enable */
      }

      this.legend.eq(i).text(series.label.replace(/=.*/, `= ${y.toFixed(2)}`));
    }
  }

  render() {
    return (
      <div>
        <div ref={(r) => { this.$chartContainer = $(r); }} style={{ height: '140px' }} />
      </div>
    );
  }
}

export default TrackingChart;
