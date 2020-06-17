using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using System.Data.Common;

namespace ProiectCasa
{
    class Program
    {
        static Beam AddColumn(Point point, double height)
        {
            var column = new Beam(Beam.BeamTypeEnum.COLUMN)
            {
                StartPoint = point,
                EndPoint = new Point(point.X, point.Y, height),
                Profile = new Profile { ProfileString = "D200" }
            };
            column.Insert();
            return column;
        }
        static ContourPlate AddPlate(List<Point> points)
        {
            var contourPlate = new ContourPlate();
            foreach(var point in points)
            {
                var contourPoint = new ContourPoint(point, null);
                contourPlate.AddContourPoint(contourPoint);
            }
            contourPlate.Profile.ProfileString = "PL100";
            contourPlate.Material.MaterialString = "A36";
            contourPlate.Insert();
            return contourPlate;
        }
        static ContourPlate AddCutPlate(List<Point> points)
        {
            var cutPlate = new ContourPlate();
            foreach(var point in points)
            {
                var cutPoint = new ContourPoint(point, null);
                cutPlate.AddContourPoint(cutPoint);
            }
            cutPlate.Profile.ProfileString = "PL100";
            cutPlate.Material.MaterialString = "A36";
            cutPlate.Class = BooleanPart.BooleanOperativeClassName;
            cutPlate.Insert();
            return cutPlate;
        }
        static void Main(string[] args)
        {
            var model = new Model();
            if(model.GetConnectionStatus())
            {
                Console.WriteLine("You can start writting your Tekla project");

                //insert fence handles
                var beamHandle1 = new Beam()
                {
                    StartPoint = new Point(0.0, 0.0, 3000.0),
                    EndPoint = new Point(36000.0, 0.0, 3000.0),
                    Profile = new Profile { ProfileString = "RHS200*5" }
                };
                beamHandle1.Insert();
                var beamHandle2 = new Beam()
                {
                    StartPoint = new Point(0.0, 0.0, 3000.0),
                    EndPoint = new Point(0.0, 30000.0, 3000.0),
                    Profile = new Profile { ProfileString = "RHS200*5" }
                };
                beamHandle2.Insert();
                var beamHandle3 = new Beam()
                {
                    StartPoint = new Point(36000.0, 0.0, 3000.0),
                    EndPoint = new Point(36000.0, 30000.0, 3000.0),
                    Profile = new Profile { ProfileString = "RHS200*5" }
                };
                beamHandle3.Insert();
                var beamHandle4 = new Beam()
                {
                    StartPoint = new Point(0.0, 30000.0, 3000.0),
                    EndPoint = new Point(36000.0, 30000.0, 3000.0),
                    Profile = new Profile { ProfileString = "RHS200*5" }
                };
                beamHandle4.Insert();

                //insert fence columns
                for (int index=0;index<=36000;index+=3000)
                {
                    var point = new Point(index * 1.0, 0.0);
                    var column = AddColumn(point, 3000);
                }
                for (int index = 3000; index <= 30000; index += 3000)
                {
                    var point = new Point(0.0, index * 1.0);
                    var column = AddColumn(point, 3000);
                }
                for (int index = 3000; index <= 36000; index += 3000)
                {
                    var point = new Point(index * 1.0, 30000.0);
                    var column = AddColumn(point, 3000);
                }
                for (int index = 3000; index <= 27000; index += 3000)
                {
                    var point = new Point(36000.0, index * 1.0);
                    var column = AddColumn(point, 3000);
                }

                Console.WriteLine("Give the weight of the house, number between 10 and 34:");
                string val = Console.ReadLine();
                int houseWeight = Convert.ToInt32(val);
                Console.WriteLine("Give the height of the house, number between 3 and 10");
                string val1= Console.ReadLine();
                int houseHeight = Convert.ToInt32(val1);
                Console.WriteLine("Give the side weight of the house, number between 10 and 28");
                string val2 = Console.ReadLine();
                int houseSideWeight = Convert.ToInt32(val2);

                //int houseSideWeight = 20;
                //int houseWeight = 25;
                //int houseHeight = 6;
                var frontWall = AddPlate(new List<Point>
                {
                    new Point(1000,1000,0),
                    new Point(houseWeight*1000,1000,0),
                    new Point(houseWeight*1000,1000,houseHeight*1000),
                    new Point(1000,1000,houseHeight*1000)
                });
                var leftWall = AddPlate(new List<Point>
                {
                    new Point(1000,1000,0),
                    new Point(1000,houseSideWeight*1000,0),
                    new Point(1000,houseSideWeight*1000,houseHeight*1000),
                    new Point(1000,1000,houseHeight*1000)
                });
                var rightWall = AddPlate(new List<Point>
                {
                    new Point(houseWeight*1000,1000,0),
                    new Point(houseWeight*1000,houseSideWeight*1000,0),
                    new Point(houseWeight*1000,houseSideWeight*1000,houseHeight*1000),
                    new Point(houseWeight*1000,1000,houseHeight*1000)
                });
                var backWall = AddPlate(new List<Point>
                {
                    new Point(1000,houseSideWeight*1000,0),
                    new Point(houseWeight*1000,houseSideWeight*1000,0),
                    new Point(houseWeight*1000,houseSideWeight*1000,houseHeight*1000),
                    new Point(1000,houseSideWeight*1000,houseHeight*1000)
                });
                var Floor = AddPlate(new List<Point>
                {
                    new Point(1000,1000,0),
                    new Point(houseWeight*1000,1000,0),
                    new Point(houseWeight*1000,houseSideWeight*1000,0),
                    new Point(1000,houseSideWeight*1000,0)
                });
                var Roof = AddPlate(new List<Point>
                {
                    new Point(1000,1000,houseHeight*1000),
                    new Point(houseWeight*1000,1000,houseHeight*1000),
                    new Point(houseWeight*1000,houseSideWeight*1000,houseHeight*1000),
                    new Point(1000,houseSideWeight*1000,houseHeight*1000)
                });
                var leftSideRoof = AddPlate(new List<Point>
                {
                    new Point(1000,1000,houseHeight*1000),
                    new Point((houseWeight/2)*1000,1000,(houseHeight+(houseHeight/2))*1000),
                    new Point((houseWeight/2)*1000,houseSideWeight*1000,(houseHeight+(houseHeight/2))*1000),
                    new Point(1000,houseSideWeight*1000,houseHeight*1000)
                });
                var RightSideRoof = AddPlate(new List<Point>
                {
                    new Point(houseWeight*1000,1000,houseHeight*1000),
                    new Point((houseWeight/2)*1000,1000,(houseHeight+(houseHeight/2))*1000),
                    new Point((houseWeight/2)*1000,houseSideWeight*1000,(houseHeight+(houseHeight/2))*1000),
                    new Point(houseWeight*1000,houseSideWeight*1000,houseHeight*1000)
                });
                var frontRoofWall = AddPlate(new List<Point>
                {
                    new Point(1000,1000,houseHeight*1000),
                    new Point((houseWeight/2)*1000,1000,(houseHeight+(houseHeight/2))*1000),
                    new Point(houseWeight*1000,1000,houseHeight*1000)
                });
                var backRoofWall = AddPlate(new List<Point>
                {
                    new Point(1000,houseSideWeight*1000,houseHeight*1000),
                    new Point((houseWeight/2)*1000,houseSideWeight*1000,(houseHeight+(houseHeight/2))*1000),
                    new Point(houseWeight*1000,houseSideWeight*1000,houseHeight*1000)
                });

                var cutDoor = AddCutPlate(new List<Point>
                {
                    new Point((houseWeight/2)*1000-1000,1000,50),
                    new Point((houseWeight/2)*1000+1000,1000,50),
                    new Point((houseWeight/2)*1000+1000,1000,(houseHeight/2)*1000),
                    new Point((houseWeight/2)*1000-1000,1000,(houseHeight/2)*1000)
                });
                var boolPart1 = new BooleanPart { Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT, Father = frontWall };
                boolPart1.SetOperativePart(cutDoor);
                boolPart1.Insert();
                //cutDoor.Delete();

                var cutBackDoor = AddCutPlate(new List<Point>
                {
                    new Point((houseWeight/2)*1000-1000,houseSideWeight*1000,50),
                    new Point((houseWeight/2)*1000+1000,houseSideWeight*1000,50),
                    new Point((houseWeight/2)*1000+1000,houseSideWeight*1000,(houseHeight/2)*1000),
                    new Point((houseWeight/2)*1000-1000,houseSideWeight*1000,(houseHeight/2)*1000)
                });
                var boolPart3 = new BooleanPart { Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT, Father = backWall };
                boolPart3.SetOperativePart(cutBackDoor);
                boolPart3.Insert();

                var cutLeftWindow = AddCutPlate(new List<Point>
                {
                    new Point(3000,1000,2000),
                    new Point(5000,1000,2000),
                    new Point(5000,1000,houseHeight*1000-1000),
                    new Point(3000,1000,houseHeight*1000-1000)
                });
                var boolPart2 = new BooleanPart { Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT, Father = frontWall };
                boolPart2.SetOperativePart(cutLeftWindow);
                boolPart2.Insert();
                cutLeftWindow.Delete();

                var cutRightWindow = AddCutPlate(new List<Point>
                {
                    new Point(houseWeight*1000-4000,1000,2000),
                    new Point(houseWeight*1000-2000,1000,2000),
                    new Point(houseWeight*1000-2000,1000,houseHeight*1000-1000),
                    new Point(houseWeight*1000-4000,1000,houseHeight*1000-1000)
                });
                var boolPart4 = new BooleanPart { Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT, Father = frontWall };
                boolPart4.SetOperativePart(cutRightWindow);
                boolPart4.Insert();
                cutRightWindow.Delete();

                model.CommitChanges();
            }
            else
            {
                Console.WriteLine("Please open Tekla first!");
            }
            Console.ReadLine();
        }
    }
}
