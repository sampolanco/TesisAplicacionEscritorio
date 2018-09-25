#pragma once
#include <windows.h>
#include <tchar.h> 
#include <stdio.h>
#include <strsafe.h>
#pragma comment(lib, "User32.lib")


//#pragma comment(lib, "User32.lib")

#include "stdio.h"
#include <string>
#include <iostream>
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include <opencv2/opencv.hpp>
using namespace cv;
using namespace std;

Mat original, imagen, imagenCopia, imagenEnmascaramientoGris, Imagenthreshold,
		ImagenthresholdCopia, elementoErosion, imagenConMangosDetectados, drawing, imagenContornos;

int quitarRedundancia() {

	return 1;
}
int procesamiento() {
	try {
		std::string direccion = "C:\\Users\\Samuel\\Desktop\\pruebaImagenes\\antracnosis";
		std::string nombre = "_(61).jpg";
		std:string numeroImagen;
		double factorInferior = .05, factorSuperior = 0.6, valorGris = 0;
		vector<vector<Point> > contours;
		//vector<Vec4i> hierarchy;
		//Numero(nombre) de mango detectado
		char nombreAux[20];
		//Imagenes recortadas
		Mat imagenRecortadaFondoBN, imagenNormalRecortada, croppedImage3;
		//Tamaño de imagenes
		Size VGA(640, 480), sizeRecorte(150, 225);//Estandar VGA, size2(15, 30),

		double escala = 3, ejex = 0, ejey = 0,pendiente1=0, pendiente2=0, pendiente3=0,primerPendiente=0,segundaPendiente=0;
		//ErosionOriginal=2
		int erosion_size = 4, separacionPixeles = 5, denomidadorPendiente = 0,contadorCambiosPendiente=0,numeroContornos=0;

		RNG rng(12345);

		HANDLE hFind = INVALID_HANDLE_VALUE;
		DWORD dwError = 0;
		double dRg = 0, dRb = 0, RRg = 0, RRb = 0;

		//Se lee la imagen de la carpeta
		original = imread(direccion + "\\" + nombre, IMREAD_COLOR); // Read the file
																	//original = imread("C:\\Users\\Samuel\\Desktop\\TT2\\naranja\\DSCN0143.jpg", IMREAD_COLOR);
		if (!original.data) // Check for invalid input
		{
			return -1;
		}
		
		//Cambiar tamaño imagen al estandar VGA
		resize(original, imagen, VGA);//resize image
									  //Se obtiene una imagen copia para realizar recortes
		imagenCopia = imagen.clone();
		//namedWindow("borrar1", WINDOW_AUTOSIZE); // Create a window for display.
		//imshow("borrar1", imagenCopia); // Show our image inside it.
		//waitKey(0);
		//Para todas las filas y columnas de la imagen
		for (int i = 0; i < imagen.rows; i++) {
			for (int j = 0; j < imagen.cols; j++) {
				//Se obtienen los indices de color del rojo con respecto al verde y azul
				dRg = imagen.at<Vec3b>(i, j)[2] - imagen.at<Vec3b>(i, j)[1];
				dRb = imagen.at<Vec3b>(i, j)[2] - imagen.at<Vec3b>(i, j)[0];
				RRg = 3 * dRg / (imagen.at<Vec3b>(i, j)[0] + imagen.at<Vec3b>(i, j)[1] + imagen.at<Vec3b>(i, j)[2]);
				RRb = 3 * dRb / (imagen.at<Vec3b>(i, j)[0] + imagen.at<Vec3b>(i, j)[1] + imagen.at<Vec3b>(i, j)[2]);
				//Si el indice de nivel de rojo esta fuera de los limites se pinta el fondo de negro
				if (RRg < factorInferior || RRb < factorInferior || RRg > factorSuperior || RRb > factorSuperior) {
					imagenCopia.at<Vec3b>(i, j)[0] = 0;
					imagenCopia.at<Vec3b>(i, j)[1] = 0;
					imagenCopia.at<Vec3b>(i, j)[2] = 0;
				}
			}
		}
		//---------------------------------------------------------------------Cambio a escala de gris
		cvtColor(imagenCopia, imagenEnmascaramientoGris, CV_BGR2GRAY);
		//---------------------------------------------------------------------Pintar de negro el fondo
		//Para todas las filas y columnas de la imagen
		for (int i = 0; i < imagen.rows; i++) {
			for (int j = 0; j < imagen.cols; j++) {
				//Se obtiene el valor de gris del pixel
				valorGris = imagenEnmascaramientoGris.at<uchar>(i, j);
				//Si el valor de intensidad es menor a 110 se pinta el fondo de negro
				if (valorGris < 110) {
					/*imagenCopia.at<Vec3b>(i, j)[0] = 0;
					imagenCopia.at<Vec3b>(i, j)[1] = 0;
					imagenCopia.at<Vec3b>(i, j)[2] = 0;*/
					imagenEnmascaramientoGris.at<uchar>(i, j) = 0;
				}
			}
		}
		//---------------------------------------------------------------------Threshold
		//Se realiza el Threshold
		threshold(imagenEnmascaramientoGris, Imagenthreshold, 110, 255, THRESH_BINARY);
		//---------------------------------------------------------------------Erosion
		//elementoErosion = getStructuringElement(MORPH_ELLIPSE, Size(2 * erosion_size + 1, 2 * erosion_size + 1), Point(erosion_size, erosion_size));
		//erode(Imagenthreshold, Imagenthreshold, elementoErosion);
		//---------------------------------------------------------------------Buscar contornos
		//	namedWindow("borrar2", WINDOW_AUTOSIZE); // Create a window for display.
		//	imshow("borrar2", Imagenthreshold); // Show our image inside it.
		//	waitKey(0);
		ImagenthresholdCopia = Imagenthreshold.clone();
		//Se obtienen los contornos de la imagen (Imagenthreshold)
		findContours(Imagenthreshold, contours, CV_RETR_TREE, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));
		//Se declaran variables
		vector<vector<Point> > contours_poly(contours.size());
		Rect boundRect;
		RotatedRect minEllipse;
		//---------------------------------------------------------------------Buscar mangos en los contornos
		//Archivo para guardar datos del contorno
		std::fstream fs;
		fs.open(direccion + "\\datos.txt");

		//para cada uno delos objetos contorno
		drawing = Mat::zeros(Imagenthreshold.size(), CV_8UC3);
		printf("\nContornos de imagen= %d\n\n",contours.size());
		for (int i = 0; i < contours.size(); i++)
		{
			//Se genera un color para la figura
			Scalar color = Scalar(rng.uniform(0, 255), rng.uniform(0, 255), rng.uniform(0, 255));

			//si el tamaño es mayor a 100 y  menor a 500 pixeles o (300,1000),o(50,200(300))
			//Se realiza una discriminacion por el tamaño del objeto para eliminar pequeños y grandes objetos
			if (contours[i].size() > 300 && contours[i].size() < 1000)
			{
				//Se reinicia el contador para ver cambios de pendiente
				contadorCambiosPendiente = 0;
				//Se crea el rectangulo y la elipse que encierra el mango
				approxPolyDP(Mat(contours[i]), contours_poly[i], 1, true);
				boundRect = boundingRect(Mat(contours_poly[i]));
				minEllipse = fitEllipse(Mat(contours[i]));
				//Se calcula la pendiente 
				for (int j = 0; j < (contours[i].size()- separacionPixeles); j += separacionPixeles) {
					//El primer punto y se tienen que calcular la pendiente1 y pendiente2
					if (j==0) {
						denomidadorPendiente = ((double)contours[i][j].x - (double)contours[i][j + separacionPixeles].x);
						//Si la pendiente no es infinita se calcula la pendiente
						if (denomidadorPendiente != 0)
							pendiente1 = ((double)contours[i][j].y - (double)contours[i][j + separacionPixeles].y) / denomidadorPendiente;
						denomidadorPendiente = (double)contours[i][j + separacionPixeles].x - (double)contours[i][j + 2*separacionPixeles].x;
						if (denomidadorPendiente != 0)
							pendiente2 = ((double)contours[i][j+ separacionPixeles].y - (double)contours[i][j + 2*separacionPixeles].y) / denomidadorPendiente;
						//Se guardan las primeras 2 pendientes
						primerPendiente = pendiente1;
						segundaPendiente = pendiente2;
					}
					//Se calcula la pendiente 3 *Antes de los 2 ultimos puntos 
				    if (j<(contours[i].size() - 2 * separacionPixeles)){
						denomidadorPendiente = ((double)contours[i][j + 2 * separacionPixeles].x - (double)contours[i][j + 3 * separacionPixeles].x);
						if (denomidadorPendiente != 0)
							pendiente3 = ((double)contours[i][j + 2 * separacionPixeles].y - (double)contours[i][j + 3 * separacionPixeles].y) / denomidadorPendiente;
					}
					//En el penultimo punto
					else if (j>=(contours[i].size()- 2*separacionPixeles))
						pendiente3 = primerPendiente;
					//En el ultimo punto
					else if(j>(contours[i].size()- separacionPixeles))
						pendiente3 = segundaPendiente;
					//Si se presenta un cambio de signo
					if (pendiente1 > pendiente2 && pendiente2 < pendiente3) {
						contadorCambiosPendiente++;
						//printf("\nP1 X= %d,Y=%d", contours[i][j].x, contours[i][j].y);
						//printf("\n Cambio de pendiente p1= %f, p2= %f, p3= %f", pendiente1, pendiente2, pendiente3);
					}
					else if (pendiente1 < pendiente2 && pendiente2 > pendiente3) {
						contadorCambiosPendiente++;
						//printf("\nP1  X= %d,Y=%d", contours[i][j].x, contours[i][j].y);
						//printf("\n Cambio de pendiente p1= %f, p2= %f, p3= %f", pendiente1, pendiente2, pendiente3);
					}
					//Se guarda la pendiente en el archivo
					fs << pendiente1 <<",";
					printf("%d,", pendiente1);
					//Se actualizan los valores de pendiente para movernos al siguiente punto
					pendiente1 = pendiente2;
					pendiente2 = pendiente3;
				}
				printf("\nP1  X= %d,Y=%d", contours[i][0].x, contours[i][0].y);
				printf("\nImagen_contadorCambiosPendientes=%d", contadorCambiosPendiente);
				//Drawing es la variable donde se guarda el contorno
				//drawContours(drawing, contours, i, Scalar(255, 255, 255), CV_FILLED);
				drawContours(drawing, contours, i, Scalar(255, 255, 255), 1);
				rectangle(drawing, boundRect.tl(), boundRect.br(), color, 2, 8, 0);

				//Se obtiene un rectangulo que encierra el contorno
				ejex = boundRect.tl().x - boundRect.br().x;
				ejey = boundRect.tl().y - boundRect.br().y;
				ejex = ejex / ejey;
				//Se discrimina para las dimenciones del rectangulo coincidan con las dimenciones normales de un mango
				if (ejex > (1 / escala) && ejex < escala) {

					//Se recorta la seccion de la imagen sin fondo perteneciente al mango
					imagenRecortadaFondoBN = Imagenthreshold(boundRect);
					//Se cambia de tamaño la imagen recortada
					resize(imagenRecortadaFondoBN, imagenRecortadaFondoBN, sizeRecorte);
					//Se obtiene el numero de mango detectado y se convierte a char para guardar el objeto
					_itoa_s(i, nombreAux, 10);
					numeroImagen = nombreAux;
					//Se guarda el resultado obtenido en una carpeta
					imwrite(direccion + "\\salida\\" + "1" + "_" + numeroImagen + "_" + nombre, imagenRecortadaFondoBN);
					//Se pinta una elipse en la imagenCopia para mostrar donde se creo el recorte
					ellipse(imagenCopia, minEllipse, color, 2, 8);
					//Se recorta la seccion de la imagen normal
					imagenNormalRecortada = imagen(boundRect);
					//Se cambia de tamaño el recorte
					resize(imagenNormalRecortada, imagenNormalRecortada, sizeRecorte);
					//Se guarda el resultado obtenido en una carpeta
					imwrite(direccion + "\\salidaAux\\" + "1" + "_" + numeroImagen + "_" + nombre, imagenNormalRecortada);
				}
				fs << endl;
			}
		}
		//---------------------------------------------------------------------Generar imagen solo con mangos
		//Se guarda la imagen con mangos detectados
		imwrite(direccion + "/salidaAux2/" + "1" + "_" + nombre, drawing);
		//Esperar que se presione una tecla
		//cv::waitKey(0);
		fs.close();
		contours.clear();
		contours_poly.clear();
		dwError = GetLastError();
		FindClose(hFind);
		return 1;
	}
	catch (exception e) {

		return 0;
	}
}

