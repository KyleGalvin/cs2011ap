#ifndef _POINTER_H_
#define _POINTER_H_

#include "SDL/SDL.h"

class Pointer
{
	private:
	int x,y;
	public:
	int getx();
	int gety();
	void move(SDL_Event*);
};

#endif
