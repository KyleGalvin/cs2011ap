#include "TyphoonCollision/geotypes.hpp"
#include "SDL/SDL.h"
#include "render.hpp"
#include "timer.hpp"
#include "TyphoonCollision/hgrid.hpp"
#include "TyphoonCollision/objectstructs.hpp"
#include "pointer.hpp"
#include <iostream>
#include "sdl-collide/SDL_collide.h"
using namespace std;
using namespace boost;
int main(){
	const int screen_w = 1024;
	const int screen_h = 768;
	const int screen_bpp = 32;
	const int fps = 60;

	SDL_Surface* screen;
	SDL_Surface* background;
	SDL_Surface* red;
	SDL_Surface* blue;

	SDL_Event event;
	
	shared_ptr<AABB> player1(new AABB());
	player1->Location.push_back(100);
	player1->Location.push_back(100);
	player1->Dimensions.push_back(64);
	player1->Dimensions.push_back(64);

	shared_ptr<AABB> player2(new AABB());
	player2->Location.push_back(300);
	player2->Location.push_back(300);
	player2->Velocity.push_back(0);
	player2->Velocity.push_back(0);
	player2->Dimensions.push_back(102);
	player2->Dimensions.push_back(64);

	Timer timer;
	bool quit = false;
	Pointer mouse(blue);

	SDL_Render::initscreen(screen_w,screen_h,screen_bpp,&screen);
	SDL_Render::loadimage("Images/testbackimage.jpg",&background);
	SDL_Render::loadimage("Images/Red.png",&(player2->Texture));
	SDL_Render::loadimage("Images/Blue.png",&(mouse.icon));
	

	//game loop
	while(!quit){
		timer.start();

		while(SDL_PollEvent(&event)){
			if(event.type==SDL_QUIT){
				quit=true;
			}else if(event.type==SDL_KEYDOWN){
				if(event.key.keysym.sym == SDLK_LEFT){
					player2->Velocity[0]-=5;
				}else if(event.key.keysym.sym == SDLK_RIGHT){
					player2->Velocity[0]+=5;
				}else if(event.key.keysym.sym == SDLK_UP){
					player2->Velocity[1]-=5;
				}else if(event.key.keysym.sym == SDLK_DOWN){
					player2->Velocity[1]+=5;
				}
			}else if(event.type==SDL_KEYUP){
				if(event.key.keysym.sym == SDLK_LEFT){
					player2->Velocity[0]+=5;
				}else if(event.key.keysym.sym == SDLK_RIGHT){
					player2->Velocity[0]-=5;
				}else if(event.key.keysym.sym == SDLK_UP){
					player2->Velocity[1]+=5;
				}else if(event.key.keysym.sym == SDLK_DOWN){
					player2->Velocity[1]-=5;
				}
			}

			//update mouse info once we have pointer set up
			mouse.move(&event);
		}

		player2->Location[0]+=player2->Velocity[0];
		player2->Location[1]+=player2->Velocity[1];

		SDL_Render::mergesurface(0,0,&background,&screen);
		SDL_Render::mergesurface(mouse.x,mouse.y,&(mouse.icon),&screen);
		SDL_Render::mergesurface(player2->Location[0],player2->Location[1],&(player2->Texture),&screen);

		if(SDL_Flip(screen)==-1){
			cout<<"Screen failed to flip\n";
		}

		//throttle framerate
		while(timer.get_ticks()<1000/fps){}
	}

	return 0;
}
