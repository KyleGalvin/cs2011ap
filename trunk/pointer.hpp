#include "SDL/SDL.h"

class Pointer
{
	public:
	int x,y,h,w;
	SDL_Surface *icon;

	Pointer(){}
	Pointer(SDL_Surface *newicon){
		h = newicon->h;
		w = newicon->w;
		icon = newicon;
	}

	void move(SDL_Event *event){
		if (event->type==SDL_MOUSEMOTION){
			x=event->button.x;
			y=event->button.y;
		}
	}
};

