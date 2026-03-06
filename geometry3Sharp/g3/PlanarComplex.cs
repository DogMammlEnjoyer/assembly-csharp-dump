using System;
using System.Collections.Generic;

namespace g3
{
	public class PlanarComplex
	{
		public PlanarComplex()
		{
			this.vElements = new List<PlanarComplex.Element>();
		}

		public int ElementCount
		{
			get
			{
				return this.vElements.Count;
			}
		}

		public PlanarComplex.Element Add(IParametricCurve2d curve)
		{
			int num;
			if (curve.IsClosed)
			{
				PlanarComplex.SmoothLoopElement smoothLoopElement = new PlanarComplex.SmoothLoopElement();
				PlanarComplex.Element element = smoothLoopElement;
				num = this.id_generator;
				this.id_generator = num + 1;
				element.ID = num;
				smoothLoopElement.source = curve;
				this.UpdateSampling(smoothLoopElement);
				this.vElements.Add(smoothLoopElement);
				return smoothLoopElement;
			}
			PlanarComplex.SmoothCurveElement smoothCurveElement = new PlanarComplex.SmoothCurveElement();
			PlanarComplex.Element element2 = smoothCurveElement;
			num = this.id_generator;
			this.id_generator = num + 1;
			element2.ID = num;
			smoothCurveElement.source = curve;
			this.UpdateSampling(smoothCurveElement);
			this.vElements.Add(smoothCurveElement);
			return smoothCurveElement;
		}

		public PlanarComplex.Element Add(Polygon2d poly)
		{
			PlanarComplex.SmoothLoopElement smoothLoopElement = new PlanarComplex.SmoothLoopElement();
			PlanarComplex.Element element = smoothLoopElement;
			int num = this.id_generator;
			this.id_generator = num + 1;
			element.ID = num;
			smoothLoopElement.source = new Polygon2DCurve
			{
				Polygon = poly
			};
			smoothLoopElement.polygon = new Polygon2d(poly);
			this.vElements.Add(smoothLoopElement);
			return smoothLoopElement;
		}

		public PlanarComplex.Element Add(PolyLine2d pline)
		{
			PlanarComplex.SmoothCurveElement smoothCurveElement = new PlanarComplex.SmoothCurveElement();
			PlanarComplex.Element element = smoothCurveElement;
			int num = this.id_generator;
			this.id_generator = num + 1;
			element.ID = num;
			smoothCurveElement.source = new PolyLine2DCurve
			{
				Polyline = pline
			};
			smoothCurveElement.polyLine = new PolyLine2d(pline);
			this.vElements.Add(smoothCurveElement);
			return smoothCurveElement;
		}

		public void Remove(PlanarComplex.Element e)
		{
			this.vElements.Remove(e);
		}

		private void UpdateSampling(PlanarComplex.SmoothCurveElement c)
		{
			if (this.MinimizeSampling && c.source is Segment2d)
			{
				c.polyLine = new PolyLine2d();
				c.polyLine.AppendVertex(((Segment2d)c.source).P0);
				c.polyLine.AppendVertex(((Segment2d)c.source).P1);
				return;
			}
			c.polyLine = new PolyLine2d(CurveSampler2.AutoSample(c.source, this.DistanceAccuracy, this.SpacingT));
		}

		private void UpdateSampling(PlanarComplex.SmoothLoopElement l)
		{
			l.polygon = new Polygon2d(CurveSampler2.AutoSample(l.source, this.DistanceAccuracy, this.SpacingT));
		}

		public void Reverse(PlanarComplex.SmoothCurveElement c)
		{
			c.source.Reverse();
			this.UpdateSampling(c);
		}

		public IEnumerable<ComplexSegment2d> AllSegmentsItr()
		{
			foreach (PlanarComplex.Element e in this.vElements)
			{
				ComplexSegment2d s = default(ComplexSegment2d);
				if (e is PlanarComplex.SmoothLoopElement)
				{
					s.isClosed = true;
				}
				else if (e is PlanarComplex.SmoothCurveElement)
				{
					s.isClosed = false;
				}
				foreach (Segment2d seg in e.SegmentItr())
				{
					s.seg = seg;
					s.element = e;
					yield return s;
				}
				IEnumerator<Segment2d> enumerator2 = null;
				s = default(ComplexSegment2d);
				e = null;
			}
			List<PlanarComplex.Element>.Enumerator enumerator = default(List<PlanarComplex.Element>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<PlanarComplex.Element> ElementsItr()
		{
			foreach (PlanarComplex.Element element in this.vElements)
			{
				yield return element;
			}
			List<PlanarComplex.Element>.Enumerator enumerator = default(List<PlanarComplex.Element>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<PlanarComplex.SmoothLoopElement> LoopsItr()
		{
			foreach (PlanarComplex.Element element in this.vElements)
			{
				if (element is PlanarComplex.SmoothLoopElement)
				{
					yield return element as PlanarComplex.SmoothLoopElement;
				}
			}
			List<PlanarComplex.Element>.Enumerator enumerator = default(List<PlanarComplex.Element>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<PlanarComplex.SmoothCurveElement> CurvesItr()
		{
			foreach (PlanarComplex.Element element in this.vElements)
			{
				if (element is PlanarComplex.SmoothCurveElement)
				{
					yield return element as PlanarComplex.SmoothCurveElement;
				}
			}
			List<PlanarComplex.Element>.Enumerator enumerator = default(List<PlanarComplex.Element>.Enumerator);
			yield break;
			yield break;
		}

		public bool HasOpenCurves()
		{
			using (List<PlanarComplex.Element>.Enumerator enumerator = this.vElements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is PlanarComplex.SmoothCurveElement)
					{
						return true;
					}
				}
			}
			return false;
		}

		public IEnumerable<IParametricCurve2d> LoopLeafComponentsItr()
		{
			foreach (PlanarComplex.Element element in this.vElements)
			{
				if (element is PlanarComplex.SmoothLoopElement)
				{
					IParametricCurve2d source = element.source;
					if (source is IMultiCurve2d)
					{
						foreach (IParametricCurve2d parametricCurve2d in CurveUtils2.LeafCurvesIteration(source))
						{
							yield return parametricCurve2d;
						}
						IEnumerator<IParametricCurve2d> enumerator2 = null;
					}
					else
					{
						yield return source;
					}
				}
			}
			List<PlanarComplex.Element>.Enumerator enumerator = default(List<PlanarComplex.Element>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<ComplexEndpoint2d> EndpointsItr()
		{
			foreach (PlanarComplex.Element element in this.vElements)
			{
				if (element is PlanarComplex.SmoothCurveElement)
				{
					PlanarComplex.SmoothCurveElement s = element as PlanarComplex.SmoothCurveElement;
					yield return new ComplexEndpoint2d
					{
						v = s.polyLine.Start,
						isStart = true,
						element = s
					};
					yield return new ComplexEndpoint2d
					{
						v = s.polyLine.End,
						isStart = false,
						element = s
					};
					s = null;
				}
			}
			List<PlanarComplex.Element>.Enumerator enumerator = default(List<PlanarComplex.Element>.Enumerator);
			yield break;
			yield break;
		}

		public AxisAlignedBox2d Bounds()
		{
			AxisAlignedBox2d empty = AxisAlignedBox2d.Empty;
			foreach (PlanarComplex.Element element in this.vElements)
			{
				empty.Contain(element.Bounds());
			}
			return empty;
		}

		public void SplitAllLoops()
		{
			List<PlanarComplex.Element> list = new List<PlanarComplex.Element>();
			List<IParametricCurve2d> list2 = new List<IParametricCurve2d>();
			foreach (PlanarComplex.SmoothLoopElement smoothLoopElement in this.LoopsItr())
			{
				if (smoothLoopElement.source is IMultiCurve2d)
				{
					list.Add(smoothLoopElement);
					this.find_sub_elements(smoothLoopElement.source as IMultiCurve2d, list2);
				}
			}
			foreach (PlanarComplex.Element e in list)
			{
				this.Remove(e);
			}
			foreach (IParametricCurve2d curve in list2)
			{
				this.Add(curve);
			}
		}

		private void find_sub_elements(IMultiCurve2d multicurve, List<IParametricCurve2d> vAdd)
		{
			foreach (IParametricCurve2d parametricCurve2d in multicurve.Curves)
			{
				if (parametricCurve2d is IMultiCurve2d)
				{
					this.find_sub_elements(parametricCurve2d as IMultiCurve2d, vAdd);
				}
				else
				{
					vAdd.Add(parametricCurve2d);
				}
			}
		}

		public bool JoinElements(ComplexEndpoint2d a, ComplexEndpoint2d b, double loop_tolerance = 1E-08)
		{
			if (a.element == b.element)
			{
				throw new Exception("PlanarComplex.ChainElements: same curve!!");
			}
			PlanarComplex.SmoothCurveElement element = a.element;
			PlanarComplex.SmoothCurveElement element2 = b.element;
			PlanarComplex.SmoothCurveElement smoothCurveElement = null;
			if (!a.isStart && b.isStart)
			{
				this.vElements.Remove(element2);
				this.append(element, element2);
				smoothCurveElement = element;
			}
			else if (a.isStart && !b.isStart)
			{
				this.vElements.Remove(element);
				this.append(element2, element);
				smoothCurveElement = element2;
			}
			else if (!a.isStart)
			{
				element2.source.Reverse();
				this.vElements.Remove(element2);
				this.append(element, element2);
				smoothCurveElement = element;
			}
			else if (a.isStart)
			{
				element.source.Reverse();
				this.vElements.Remove(element2);
				this.append(element, element2);
				smoothCurveElement = element;
			}
			if (smoothCurveElement != null)
			{
				if ((smoothCurveElement.polyLine.Start - smoothCurveElement.polyLine.End).Length < loop_tolerance)
				{
					if (!(smoothCurveElement.source is ParametricCurveSequence2))
					{
						throw new Exception("PlanarComplex.JoinElements: we have closed a loop but it is not a parametric seq??");
					}
					(smoothCurveElement.source as ParametricCurveSequence2).IsClosed = true;
					PlanarComplex.SmoothLoopElement smoothLoopElement = new PlanarComplex.SmoothLoopElement();
					int num = this.id_generator;
					this.id_generator = num + 1;
					smoothLoopElement.ID = num;
					smoothLoopElement.source = smoothCurveElement.source;
					PlanarComplex.SmoothLoopElement smoothLoopElement2 = smoothLoopElement;
					this.vElements.Remove(smoothCurveElement);
					this.vElements.Add(smoothLoopElement2);
					this.UpdateSampling(smoothLoopElement2);
				}
				return true;
			}
			return false;
		}

		public void ConvertToLoop(PlanarComplex.SmoothCurveElement curve, double tolerance = 1E-08)
		{
			if ((curve.polyLine.Start - curve.polyLine.End).Length < tolerance)
			{
				if (curve.polyLine.VertexCount == 2)
				{
					this.vElements.Remove(curve);
					return;
				}
				if (!(curve.source is ParametricCurveSequence2))
				{
					throw new Exception("PlanarComplex.ConvertToLoop: we have closed a loop but it is not a parametric seq??");
				}
				(curve.source as ParametricCurveSequence2).IsClosed = true;
				PlanarComplex.SmoothLoopElement smoothLoopElement = new PlanarComplex.SmoothLoopElement();
				int num = this.id_generator;
				this.id_generator = num + 1;
				smoothLoopElement.ID = num;
				smoothLoopElement.source = curve.source;
				PlanarComplex.SmoothLoopElement smoothLoopElement2 = smoothLoopElement;
				this.vElements.Remove(curve);
				this.vElements.Add(smoothLoopElement2);
				this.UpdateSampling(smoothLoopElement2);
			}
		}

		private void append(PlanarComplex.SmoothCurveElement cTo, PlanarComplex.SmoothCurveElement cAppend)
		{
			ParametricCurveSequence2 parametricCurveSequence = null;
			if (cTo.source is ParametricCurveSequence2)
			{
				parametricCurveSequence = (cTo.source as ParametricCurveSequence2);
			}
			else
			{
				parametricCurveSequence = new ParametricCurveSequence2();
				parametricCurveSequence.Append(cTo.source);
			}
			if (cAppend.source is ParametricCurveSequence2)
			{
				using (IEnumerator<IParametricCurve2d> enumerator = (cAppend.source as ParametricCurveSequence2).Curves.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IParametricCurve2d c = enumerator.Current;
						parametricCurveSequence.Append(c);
					}
					goto IL_82;
				}
			}
			parametricCurveSequence.Append(cAppend.source);
			IL_82:
			cTo.source = parametricCurveSequence;
			this.UpdateSampling(cTo);
		}

		public PlanarComplex.SolidRegionInfo FindSolidRegions(double fSimplifyDeviationTol = 0.1, bool bWantCurveSolids = true)
		{
			PlanarComplex.FindSolidsOptions @default = PlanarComplex.FindSolidsOptions.Default;
			@default.SimplifyDeviationTolerance = fSimplifyDeviationTol;
			@default.WantCurveSolids = bWantCurveSolids;
			return this.FindSolidRegions(@default);
		}

		public PlanarComplex.SolidRegionInfo FindSolidRegions(PlanarComplex.FindSolidsOptions options)
		{
			List<PlanarComplex.SmoothLoopElement> list = new List<PlanarComplex.SmoothLoopElement>(this.LoopsItr());
			int count = list.Count;
			int num = 0;
			foreach (PlanarComplex.SmoothLoopElement smoothLoopElement in list)
			{
				num = Math.Max(num, smoothLoopElement.ID + 1);
			}
			AxisAlignedBox2d[] bounds = new AxisAlignedBox2d[num];
			foreach (PlanarComplex.SmoothLoopElement smoothLoopElement2 in list)
			{
				bounds[smoothLoopElement2.ID] = smoothLoopElement2.Bounds();
			}
			double num2 = 0.0;
			double simplifyDeviationTolerance = options.SimplifyDeviationTolerance;
			Polygon2d[] array = new Polygon2d[num];
			foreach (PlanarComplex.SmoothLoopElement smoothLoopElement3 in list)
			{
				Polygon2d polygon2d = new Polygon2d(smoothLoopElement3.polygon);
				if (num2 > 0.0 || simplifyDeviationTolerance > 0.0)
				{
					polygon2d.Simplify(num2, simplifyDeviationTolerance, true);
				}
				array[smoothLoopElement3.ID] = polygon2d;
			}
			list.Sort(delegate(PlanarComplex.SmoothLoopElement x, PlanarComplex.SmoothLoopElement y)
			{
				if (!bounds[x.ID].Contains(bounds[y.ID]))
				{
					return 1;
				}
				return -1;
			});
			bool[] array2 = new bool[count];
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			Dictionary<int, List<int>> dictionary2 = new Dictionary<int, List<int>>();
			bool trustOrientations = options.TrustOrientations;
			bool wantCurveSolids = options.WantCurveSolids;
			bool bCheckContainment = !options.AllowOverlappingHoles;
			for (int i = 0; i < count; i++)
			{
				PlanarComplex.SmoothLoopElement smoothLoopElement4 = list[i];
				Polygon2d polygon2d2 = array[smoothLoopElement4.ID];
				for (int j = 0; j < count; j++)
				{
					if (i != j)
					{
						PlanarComplex.SmoothLoopElement smoothLoopElement5 = list[j];
						Polygon2d o = array[smoothLoopElement5.ID];
						if ((!trustOrientations || smoothLoopElement5.polygon.IsClockwise != smoothLoopElement4.polygon.IsClockwise) && bounds[smoothLoopElement4.ID].Contains(bounds[smoothLoopElement5.ID]) && polygon2d2.Contains(o))
						{
							if (!dictionary.ContainsKey(i))
							{
								dictionary.Add(i, new List<int>());
							}
							dictionary[i].Add(j);
							array2[j] = true;
							if (!dictionary2.ContainsKey(j))
							{
								dictionary2.Add(j, new List<int>());
							}
							dictionary2[j].Add(i);
						}
					}
				}
			}
			List<GeneralPolygon2d> list2 = new List<GeneralPolygon2d>();
			List<PlanarComplex.GeneralSolid> list3 = new List<PlanarComplex.GeneralSolid>();
			List<PlanarSolid2d> list4 = new List<PlanarSolid2d>();
			HashSet<PlanarComplex.SmoothLoopElement> hashSet = new HashSet<PlanarComplex.SmoothLoopElement>();
			Dictionary<PlanarComplex.SmoothLoopElement, int> dictionary3 = new Dictionary<PlanarComplex.SmoothLoopElement, int>();
			List<int> list5 = new List<int>();
			for (int k = 0; k < count; k++)
			{
				PlanarComplex.SmoothLoopElement smoothLoopElement6 = list[k];
				if (!array2[k])
				{
					Polygon2d polygon2d3 = array[smoothLoopElement6.ID];
					IParametricCurve2d parametricCurve2d = wantCurveSolids ? smoothLoopElement6.source.Clone() : null;
					if (!polygon2d3.IsClockwise)
					{
						polygon2d3.Reverse();
						if (wantCurveSolids)
						{
							parametricCurve2d.Reverse();
						}
					}
					GeneralPolygon2d generalPolygon2d = new GeneralPolygon2d();
					generalPolygon2d.Outer = polygon2d3;
					PlanarSolid2d planarSolid2d = new PlanarSolid2d();
					if (wantCurveSolids)
					{
						planarSolid2d.SetOuter(parametricCurve2d, true);
					}
					int count2 = list2.Count;
					dictionary3[smoothLoopElement6] = count2;
					hashSet.Add(smoothLoopElement6);
					if (dictionary.ContainsKey(k))
					{
						list5.Add(k);
					}
					list2.Add(generalPolygon2d);
					list3.Add(new PlanarComplex.GeneralSolid
					{
						Outer = smoothLoopElement6
					});
					if (wantCurveSolids)
					{
						list4.Add(planarSolid2d);
					}
				}
			}
			while (list5.Count > 0)
			{
				List<int> list6 = new List<int>();
				foreach (int num3 in list5)
				{
					PlanarComplex.SmoothLoopElement key = list[num3];
					int index = dictionary3[key];
					foreach (int num4 in dictionary[num3])
					{
						PlanarComplex.SmoothLoopElement smoothLoopElement7 = list[num4];
						if (dictionary2[num4].Count <= 1)
						{
							Polygon2d polygon2d4 = array[smoothLoopElement7.ID];
							IParametricCurve2d parametricCurve2d2 = wantCurveSolids ? smoothLoopElement7.source.Clone() : null;
							if (polygon2d4.IsClockwise)
							{
								polygon2d4.Reverse();
								if (wantCurveSolids)
								{
									parametricCurve2d2.Reverse();
								}
							}
							try
							{
								list2[index].AddHole(polygon2d4, bCheckContainment, true);
								list3[index].Holes.Add(smoothLoopElement7);
								if (parametricCurve2d2 != null)
								{
									list4[index].AddHole(parametricCurve2d2);
								}
							}
							catch
							{
							}
							hashSet.Add(smoothLoopElement7);
							if (dictionary.ContainsKey(num4))
							{
								list6.Add(num4);
							}
						}
					}
					list6.Add(num3);
				}
				foreach (int num5 in list6)
				{
					dictionary.Remove(num5);
					foreach (int key2 in new List<int>(dictionary2.Keys))
					{
						if (dictionary2[key2].Contains(num5))
						{
							dictionary2[key2].Remove(num5);
						}
					}
				}
				list5.Clear();
				for (int l = 0; l < count; l++)
				{
					PlanarComplex.SmoothLoopElement smoothLoopElement8 = list[l];
					if (!hashSet.Contains(smoothLoopElement8) && dictionary.ContainsKey(l) && dictionary2[l].Count <= 0)
					{
						Polygon2d polygon2d5 = array[smoothLoopElement8.ID];
						IParametricCurve2d parametricCurve2d3 = wantCurveSolids ? smoothLoopElement8.source.Clone() : null;
						if (!polygon2d5.IsClockwise)
						{
							polygon2d5.Reverse();
							if (wantCurveSolids)
							{
								parametricCurve2d3.Reverse();
							}
						}
						GeneralPolygon2d generalPolygon2d2 = new GeneralPolygon2d();
						generalPolygon2d2.Outer = polygon2d5;
						PlanarSolid2d planarSolid2d2 = new PlanarSolid2d();
						if (wantCurveSolids)
						{
							planarSolid2d2.SetOuter(parametricCurve2d3, true);
						}
						int count3 = list2.Count;
						dictionary3[smoothLoopElement8] = count3;
						hashSet.Add(smoothLoopElement8);
						if (dictionary.ContainsKey(l))
						{
							list5.Add(l);
						}
						list2.Add(generalPolygon2d2);
						list3.Add(new PlanarComplex.GeneralSolid
						{
							Outer = smoothLoopElement8
						});
						if (wantCurveSolids)
						{
							list4.Add(planarSolid2d2);
						}
					}
				}
			}
			for (int m = 0; m < count; m++)
			{
				PlanarComplex.SmoothLoopElement smoothLoopElement9 = list[m];
				if (!hashSet.Contains(smoothLoopElement9))
				{
					Polygon2d polygon2d6 = array[smoothLoopElement9.ID];
					IParametricCurve2d parametricCurve2d4 = wantCurveSolids ? smoothLoopElement9.source.Clone() : null;
					if (!polygon2d6.IsClockwise)
					{
						polygon2d6.Reverse();
						if (wantCurveSolids)
						{
							parametricCurve2d4.Reverse();
						}
					}
					GeneralPolygon2d generalPolygon2d3 = new GeneralPolygon2d();
					generalPolygon2d3.Outer = polygon2d6;
					PlanarSolid2d planarSolid2d3 = new PlanarSolid2d();
					if (wantCurveSolids)
					{
						planarSolid2d3.SetOuter(parametricCurve2d4, true);
					}
					list2.Add(generalPolygon2d3);
					list3.Add(new PlanarComplex.GeneralSolid
					{
						Outer = smoothLoopElement9
					});
					if (wantCurveSolids)
					{
						list4.Add(planarSolid2d3);
					}
				}
			}
			return new PlanarComplex.SolidRegionInfo
			{
				Polygons = list2,
				PolygonsSources = list3,
				Solids = (wantCurveSolids ? list4 : null)
			};
		}

		public PlanarComplex.ClosedLoopsInfo FindClosedLoops(double fSimplifyDeviationTol = 0.1)
		{
			List<PlanarComplex.SmoothLoopElement> list = new List<PlanarComplex.SmoothLoopElement>(this.LoopsItr());
			int count = list.Count;
			int num = 0;
			foreach (PlanarComplex.SmoothLoopElement smoothLoopElement in list)
			{
				num = Math.Max(num, smoothLoopElement.ID + 1);
			}
			double num2 = 0.0;
			Polygon2d[] array = new Polygon2d[num];
			IParametricCurve2d[] array2 = new IParametricCurve2d[num];
			foreach (PlanarComplex.SmoothLoopElement smoothLoopElement2 in list)
			{
				Polygon2d polygon2d = new Polygon2d(smoothLoopElement2.polygon);
				if (num2 > 0.0 || fSimplifyDeviationTol > 0.0)
				{
					polygon2d.Simplify(num2, fSimplifyDeviationTol, true);
				}
				array[smoothLoopElement2.ID] = polygon2d;
				array2[smoothLoopElement2.ID] = smoothLoopElement2.source;
			}
			PlanarComplex.ClosedLoopsInfo closedLoopsInfo = new PlanarComplex.ClosedLoopsInfo
			{
				Polygons = new List<Polygon2d>(),
				Loops = new List<IParametricCurve2d>()
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].VertexCount > 0)
				{
					closedLoopsInfo.Polygons.Add(array[i]);
					closedLoopsInfo.Loops.Add(array2[i]);
				}
			}
			return closedLoopsInfo;
		}

		public PlanarComplex.OpenCurvesInfo FindOpenCurves(double fSimplifyDeviationTol = 0.1)
		{
			List<PlanarComplex.SmoothCurveElement> list = new List<PlanarComplex.SmoothCurveElement>(this.CurvesItr());
			int count = list.Count;
			int num = 0;
			foreach (PlanarComplex.SmoothCurveElement smoothCurveElement in list)
			{
				num = Math.Max(num, smoothCurveElement.ID + 1);
			}
			double num2 = 0.0;
			PolyLine2d[] array = new PolyLine2d[num];
			IParametricCurve2d[] array2 = new IParametricCurve2d[num];
			foreach (PlanarComplex.SmoothCurveElement smoothCurveElement2 in list)
			{
				PolyLine2d polyLine2d = new PolyLine2d(smoothCurveElement2.polyLine);
				if (num2 > 0.0 || fSimplifyDeviationTol > 0.0)
				{
					polyLine2d.Simplify(num2, fSimplifyDeviationTol, true);
				}
				array[smoothCurveElement2.ID] = polyLine2d;
				array2[smoothCurveElement2.ID] = smoothCurveElement2.source;
			}
			PlanarComplex.OpenCurvesInfo openCurvesInfo = new PlanarComplex.OpenCurvesInfo
			{
				Polylines = new List<PolyLine2d>(),
				Curves = new List<IParametricCurve2d>()
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].VertexCount > 0)
				{
					openCurvesInfo.Polylines.Add(array[i]);
					openCurvesInfo.Curves.Add(array2[i]);
				}
			}
			return openCurvesInfo;
		}

		public PlanarComplex Clone()
		{
			PlanarComplex planarComplex = new PlanarComplex();
			planarComplex.DistanceAccuracy = this.DistanceAccuracy;
			planarComplex.AngleAccuracyDeg = this.AngleAccuracyDeg;
			planarComplex.SpacingT = this.SpacingT;
			planarComplex.MinimizeSampling = this.MinimizeSampling;
			planarComplex.id_generator = this.id_generator;
			planarComplex.vElements = new List<PlanarComplex.Element>(this.vElements.Count);
			foreach (PlanarComplex.Element element in this.vElements)
			{
				planarComplex.vElements.Add(element.Clone());
			}
			return planarComplex;
		}

		public void Append(PlanarComplex append)
		{
			foreach (PlanarComplex.Element element in append.vElements)
			{
				PlanarComplex.Element element2 = element;
				int num = this.id_generator;
				this.id_generator = num + 1;
				element2.ID = num;
				this.vElements.Add(element);
			}
			append.vElements.Clear();
		}

		public void Transform(ITransform2 xform, bool bApplyToSources, bool bRecomputePolygons = false)
		{
			foreach (PlanarComplex.Element element in this.vElements)
			{
				if (element is PlanarComplex.SmoothLoopElement)
				{
					PlanarComplex.SmoothLoopElement smoothLoopElement = element as PlanarComplex.SmoothLoopElement;
					if (bApplyToSources && smoothLoopElement.source != smoothLoopElement.polygon)
					{
						smoothLoopElement.source.Transform(xform);
					}
					if (bRecomputePolygons)
					{
						this.UpdateSampling(smoothLoopElement);
					}
					else
					{
						smoothLoopElement.polygon.Transform(xform);
					}
				}
				else if (element is PlanarComplex.SmoothCurveElement)
				{
					PlanarComplex.SmoothCurveElement smoothCurveElement = element as PlanarComplex.SmoothCurveElement;
					if (bApplyToSources && smoothCurveElement.source != smoothCurveElement.polyLine)
					{
						smoothCurveElement.source.Transform(xform);
					}
					if (bRecomputePolygons)
					{
						this.UpdateSampling(smoothCurveElement);
					}
					else
					{
						smoothCurveElement.polyLine.Transform(xform);
					}
				}
			}
		}

		public void PrintStats(string label = "")
		{
			Console.WriteLine("PlanarComplex Stats {0}", label);
			List<PlanarComplex.SmoothLoopElement> list = new List<PlanarComplex.SmoothLoopElement>(this.LoopsItr());
			List<PlanarComplex.SmoothCurveElement> list2 = new List<PlanarComplex.SmoothCurveElement>(this.CurvesItr());
			AxisAlignedBox2d axisAlignedBox2d = this.Bounds();
			Console.WriteLine("  Bounding Box  w: {0} h: {1}  range {2} ", axisAlignedBox2d.Width, axisAlignedBox2d.Height, axisAlignedBox2d);
			List<ComplexEndpoint2d> list3 = new List<ComplexEndpoint2d>(this.EndpointsItr());
			Console.WriteLine("  Closed Loops {0}  Open Curves {1}   Open Endpoints {2}", list.Count, list2.Count, list3.Count);
			int num = this.CountType(typeof(Segment2d));
			int num2 = this.CountType(typeof(Arc2d));
			int num3 = this.CountType(typeof(Circle2d));
			int num4 = this.CountType(typeof(NURBSCurve2));
			int num5 = this.CountType(typeof(Ellipse2d));
			int num6 = this.CountType(typeof(EllipseArc2d));
			int num7 = this.CountType(typeof(ParametricCurveSequence2));
			Console.WriteLine("  [Type Counts]   // {0} multi-curves", num7);
			Console.WriteLine("    segments {0,4}  arcs     {1,4}  circles      {2,4}", num, num2, num3);
			Console.WriteLine("    nurbs    {0,4}  ellipses {1,4}  ellipse-arcs {2,4}", num4, num5, num6);
		}

		public int CountType(Type t)
		{
			int num = 0;
			foreach (PlanarComplex.Element element in this.vElements)
			{
				if (element.source.GetType() == t)
				{
					num++;
				}
				if (element.source is IMultiCurve2d)
				{
					num += this.CountType(element.source as IMultiCurve2d, t);
				}
			}
			return num;
		}

		public int CountType(IMultiCurve2d curve, Type t)
		{
			int num = 0;
			foreach (IParametricCurve2d parametricCurve2d in curve.Curves)
			{
				if (parametricCurve2d.GetType() == t)
				{
					num++;
				}
				if (parametricCurve2d is IMultiCurve2d)
				{
					num += this.CountType(parametricCurve2d as IMultiCurve2d, t);
				}
			}
			return num;
		}

		public double DistanceAccuracy = 0.1;

		public double AngleAccuracyDeg = 5.0;

		public double SpacingT = 0.01;

		public bool MinimizeSampling;

		private int id_generator = 1;

		private List<PlanarComplex.Element> vElements;

		public abstract class Element
		{
			public Colorf Color
			{
				get
				{
					return this.color;
				}
				set
				{
					this.color = value;
					this.has_set_color = true;
				}
			}

			public bool HasSetColor
			{
				get
				{
					return this.has_set_color;
				}
			}

			protected void copy_to(PlanarComplex.Element new_element)
			{
				new_element.ID = this.ID;
				new_element.color = this.color;
				new_element.has_set_color = this.has_set_color;
				if (this.source != null)
				{
					new_element.source = this.source.Clone();
				}
			}

			public abstract IEnumerable<Segment2d> SegmentItr();

			public abstract AxisAlignedBox2d Bounds();

			public abstract PlanarComplex.Element Clone();

			public IParametricCurve2d source;

			public int ID;

			private Colorf color = Colorf.Black;

			private bool has_set_color;
		}

		public class SmoothCurveElement : PlanarComplex.Element
		{
			public override IEnumerable<Segment2d> SegmentItr()
			{
				return this.polyLine.SegmentItr();
			}

			public override AxisAlignedBox2d Bounds()
			{
				return this.polyLine.GetBounds();
			}

			public override PlanarComplex.Element Clone()
			{
				PlanarComplex.SmoothCurveElement smoothCurveElement = new PlanarComplex.SmoothCurveElement();
				base.copy_to(smoothCurveElement);
				smoothCurveElement.polyLine = ((this.polyLine == this.source) ? (smoothCurveElement.source as PolyLine2d) : new PolyLine2d(this.polyLine));
				return smoothCurveElement;
			}

			public PolyLine2d polyLine;
		}

		public class SmoothLoopElement : PlanarComplex.Element
		{
			public override IEnumerable<Segment2d> SegmentItr()
			{
				return this.polygon.SegmentItr();
			}

			public override AxisAlignedBox2d Bounds()
			{
				return this.polygon.GetBounds();
			}

			public override PlanarComplex.Element Clone()
			{
				PlanarComplex.SmoothLoopElement smoothLoopElement = new PlanarComplex.SmoothLoopElement();
				base.copy_to(smoothLoopElement);
				smoothLoopElement.polygon = ((this.polygon == this.source) ? (smoothLoopElement.source as Polygon2d) : new Polygon2d(this.polygon));
				return smoothLoopElement;
			}

			public Polygon2d polygon;
		}

		public class GeneralSolid
		{
			public PlanarComplex.Element Outer;

			public List<PlanarComplex.Element> Holes = new List<PlanarComplex.Element>();
		}

		public class SolidRegionInfo
		{
			public AxisAlignedBox2d Bounds
			{
				get
				{
					AxisAlignedBox2d empty = AxisAlignedBox2d.Empty;
					foreach (GeneralPolygon2d generalPolygon2d in this.Polygons)
					{
						empty.Contain(generalPolygon2d.Bounds);
					}
					return empty;
				}
			}

			public double Area
			{
				get
				{
					double num = 0.0;
					foreach (GeneralPolygon2d generalPolygon2d in this.Polygons)
					{
						num += generalPolygon2d.Area;
					}
					return num;
				}
			}

			public double HolesArea
			{
				get
				{
					double num = 0.0;
					foreach (GeneralPolygon2d generalPolygon2d in this.Polygons)
					{
						foreach (Polygon2d polygon2d in generalPolygon2d.Holes)
						{
							num += Math.Abs(polygon2d.SignedArea);
						}
					}
					return num;
				}
			}

			public List<GeneralPolygon2d> Polygons;

			public List<PlanarSolid2d> Solids;

			public List<PlanarComplex.GeneralSolid> PolygonsSources;
		}

		public struct FindSolidsOptions
		{
			public double SimplifyDeviationTolerance;

			public bool WantCurveSolids;

			public bool TrustOrientations;

			public bool AllowOverlappingHoles;

			public static readonly PlanarComplex.FindSolidsOptions Default = new PlanarComplex.FindSolidsOptions
			{
				SimplifyDeviationTolerance = 0.1,
				WantCurveSolids = true,
				TrustOrientations = false,
				AllowOverlappingHoles = false
			};

			public static readonly PlanarComplex.FindSolidsOptions SortPolygons = new PlanarComplex.FindSolidsOptions
			{
				SimplifyDeviationTolerance = 0.0,
				WantCurveSolids = false,
				TrustOrientations = true,
				AllowOverlappingHoles = false
			};
		}

		public class ClosedLoopsInfo
		{
			public AxisAlignedBox2d Bounds
			{
				get
				{
					AxisAlignedBox2d empty = AxisAlignedBox2d.Empty;
					foreach (Polygon2d polygon2d in this.Polygons)
					{
						empty.Contain(polygon2d.GetBounds());
					}
					return empty;
				}
			}

			public List<Polygon2d> Polygons;

			public List<IParametricCurve2d> Loops;
		}

		public class OpenCurvesInfo
		{
			public AxisAlignedBox2d Bounds
			{
				get
				{
					AxisAlignedBox2d empty = AxisAlignedBox2d.Empty;
					foreach (PolyLine2d polyLine2d in this.Polylines)
					{
						empty.Contain(polyLine2d.GetBounds());
					}
					return empty;
				}
			}

			public List<PolyLine2d> Polylines;

			public List<IParametricCurve2d> Curves;
		}
	}
}
