# DynaCity

A Software City for visualizing traces that can be used to perform root cause analyses.

Additional information about the layout of DynaCity can be found in the publication
_V. Dashuber, M. Philippsen, J. Weigend, A Layered Software City for Dependency Visualization, in: Proc. 16th Intl.
Joint Conf. on Computer Vision, Imaging and Computer Graphics Theory and Applications, Online, 2021, pp. 15–26_.

The trace visualization is published in _V. Dashuber, M. Philippsen, Trace Visualization within the Software City
Metaphor: A Controlled Experiment on Program Comprehension, in: Proc. IEEE Work. Conf. on Softw. Vis., Online, 2021, pp.
55–64_. The source code contains an extended version where the arcs and buildings are colored based on the HTTP
error code of the trace data.

### Requirements

* Unity
* C#

### Where to start

The project includes three example scenes (`BadRequest.unity, DbBroken.unity, PackageDrop.unity`) with recorded trace
data of the [Springboot Realworld Example App](https://github.com/gothinkster/spring-boot-realworld-example-app). You
can navigate through the Software City with the classical `WASD` keys as well as `Q` and `E` for vertical movement.
Additionally, you can move the camera by clicking and holding the right mouse button. By clicking on a building or
district with the left mouse button the name of the class/package is shown.

### Try it out

For trying the visualization with for own data, you need to generate a dependency graph of your application. The
required input format corresponds to
the [`jdeps`](https://docs.oracle.com/javase/8/docs/technotes/tools/unix/jdeps.html) analyses with a dot output, e.g.
under `Assets/Resources/Dot/spring-boot-realworld-example-app.dot`. The format for the spans can be seen under `
Assets/Resources/TraceData/`, it is the exported trace data from
the [Elastic stack](https://www.elastic.co/elastic-stack/). Other importers can be written too. 