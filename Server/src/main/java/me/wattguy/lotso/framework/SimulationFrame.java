/*
 * Copyright (c) 2010-2016 William Bittle  http://www.dyn4j.org/
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted 
 * provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice, this list of conditions 
 *     and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
 *     and the following disclaimer in the documentation and/or other materials provided with the 
 *     distribution.
 *   * Neither the name of dyn4j nor the names of its contributors may be used to endorse or 
 *     promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR 
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND 
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
package me.wattguy.lotso.framework;

import java.awt.*;
import java.awt.event.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import javax.swing.*;

//import javafx.scene.input.KeyCode;
import me.wattguy.lotso.Main;
import me.wattguy.lotso.utils.ColorUtilities;
import org.dyn4j.dynamics.World;
import org.dyn4j.geometry.AABB;
import org.dyn4j.geometry.Vector2;

import com.jogamp.newt.awt.NewtCanvasAWT;
import com.jogamp.newt.opengl.GLWindow;
import com.jogamp.opengl.GL;
import com.jogamp.opengl.GL2;
import com.jogamp.opengl.GLAutoDrawable;
import com.jogamp.opengl.GLCapabilities;
import com.jogamp.opengl.GLEventListener;
import com.jogamp.opengl.GLException;
import com.jogamp.opengl.GLProfile;
import com.jogamp.opengl.util.Animator;

import me.wattguy.lotso.utils.RenderUtilities;
import org.dyn4j.geometry.Vector3;

/**
 * A very VERY simple framework for building samples.
 * @since 3.2.0
 * @version 3.2.0
 */
public abstract class SimulationFrame implements GLEventListener {

	/** The conversion factor from nano to base */
	public static final double NANO_TO_BASE = 1.0e9;

	/** The canvas to draw to */
	protected final NewtCanvasAWT canvas;
	
	/** The dynamics engine */
	public final World world;
	
	/** The pixels per meter scale factor */
	public double scale;
	
	/** The time stamp for the last iteration */
	private long last;

	private Vector2 offset = new Vector2();
	private Dimension size;

	public JFrame frame;
	
	public Animator animator;
	private GLWindow window;

	/**
	 * Constructor.
	 * <p>
	 * By default creates a 800x600 canvas.
	 * @param name the frame name
	 * @param scale the pixels per meter scale factor
	 */
	public SimulationFrame(String name, double scale) {
		frame = new JFrame(name);
		
		// set the scale
		this.scale = scale;
		
		// create the world
		this.world = new World();
		
		// setup the JFrame
		frame.setDefaultCloseOperation(JFrame.HIDE_ON_CLOSE);
		
		// add a window listener
		frame.addWindowListener(new WindowAdapter() {
			/* (non-Javadoc)
			 * @see java.awt.event.WindowAdapter#windowClosing(java.awt.event.WindowEvent)
			 */
			@Override
			public void windowClosing(WindowEvent e) {
				// before we stop the JVM stop the simulation
				//stop();
				super.windowClosing(e);
			}
		});
		
		// setup OpenGL capabilities
		if (!GLProfile.isAvailable(GLProfile.GL2)) {
			throw new GLException("Version!");
		}

		GLCapabilities caps = new GLCapabilities(GLProfile.get(GLProfile.GL2));
		caps.setDoubleBuffered(true);
		// setup the stencil buffer to outline shapes
		caps.setStencilBits(1);
		// setting multisampling allows for better looking body outlines
		caps.setSampleBuffers(true);
		caps.setNumSamples(2);
		caps.setHardwareAccelerated(true);
		
		// create a NEWT window
		window = GLWindow.create(caps);
		window.setUndecorated(true);
		window.addGLEventListener(this);
		
		// create the size of the window
		size = new Dimension(800, 600);
		
		// create a canvas to paint to 
		this.canvas = new NewtCanvasAWT(window);
		this.canvas.setPreferredSize(size);
		this.canvas.setMinimumSize(size);
		this.canvas.setIgnoreRepaint(true);
		
		// add the canvas to the JFrame
		JPanel pnlTest = new JPanel();
		pnlTest.setLayout(new BorderLayout());
		pnlTest.add(this.canvas);

		frame.add(pnlTest);
		
		// make the JFrame not resizable
		// (this way I dont have to worry about resize events)
		frame.setResizable(false);
		
		// size everything
		frame.pack();

		Mouse ml = new Mouse();
		frame.addMouseWheelListener(ml);

		window.addMouseListener(ml);
		window.addKeyListener(new com.jogamp.newt.event.KeyListener() {

			@Override
			public void keyPressed(com.jogamp.newt.event.KeyEvent e) {

				if (e.getKeyCode() == KeyEvent.VK_SPACE){

					offset = new Vector2(0f, 0f);

				}

			}

			@Override
			public void keyReleased(com.jogamp.newt.event.KeyEvent e) {}

		});

		frame.setLocationRelativeTo(null);

		this.last = System.nanoTime();
		
		this.animator = new Animator(window);
		this.animator.setRunAsFastAsPossible(false);
		this.animator.start();
		
		// setup the world
		this.initializeWorld();
	}
	
	@Override
	public void init(GLAutoDrawable glDrawable) {
		// get the OpenGL context
		GL2 gl = glDrawable.getGL().getGL2();
		
		int[] temp = new int[1];
		gl.glGetIntegerv(GL.GL_STENCIL_BITS, temp, 0);
		
		// set the matrix mode to projection
		gl.glMatrixMode(GL2.GL_PROJECTION);
		// initialize the matrix
		gl.glLoadIdentity();
		// set the view to a 2D view
		gl.glOrtho(-400, 400, -300, 300, 0, 1);
		
		// switch to the model view matrix
		gl.glMatrixMode(GL2.GL_MODELVIEW);
		// initialize the matrix
		gl.glLoadIdentity();
		
		// set the clear color to white
		gl.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
		// set the stencil clear value
		gl.glClearStencil(0);
		
		// disable depth testing since we are working in 2D
		gl.glDisable(GL.GL_DEPTH_TEST);
		// we dont need lighting either
		gl.glDisable(GL2.GL_LIGHTING);
		
		// enable blending for translucency
		gl.glEnable(GL.GL_BLEND);
		gl.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
				
		// set the swap interval to vertical-sync
		gl.setSwapInterval(1);
	}
	
	@Override
	public void display(GLAutoDrawable glDrawable) {
		// get the OpenGL context
		GL2 gl = glDrawable.getGL().getGL2();

		// turn on/off multi-sampling
		gl.glEnable(GL.GL_MULTISAMPLE);
		
		// turn on/off vertical sync
		gl.setSwapInterval(1);
		
		// clear the screen
		gl.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
		// switch to the model view matrix
		gl.glMatrixMode(GL2.GL_MODELVIEW);
		// initialize the matrix (0,0) is in the center of the window
		gl.glLoadIdentity();

		// render the scene
		this.render(gl);
		this.update();
	}
	
	@Override
	public void reshape(GLAutoDrawable glDrawable, int x, int y, int width, int height) {
		// get the OpenGL context
		GL2 gl = glDrawable.getGL().getGL2();
		
		// resize the ortho view
		
		// set the matrix mode to projection
		gl.glMatrixMode(GL2.GL_PROJECTION);
		// initialize the matrix
		gl.glLoadIdentity();
		// set the view to a 2D view
		gl.glOrtho(-width / 2.0, width / 2.0, -height / 2.0, height / 2.0, 0, 1);
		
		// switch to the model view matrix
		gl.glMatrixMode(GL2.GL_MODELVIEW);
		// initialize the matrix
		gl.glLoadIdentity();
		
		// set the size
		this.size = new Dimension(width, height);
	}
	
	/**
	 * Renders the world.
	 * @param gl the OpenGL surface
	 */
	private void render(GL2 gl) {

		// apply a scaling transformation
		gl.glPushMatrix();
		gl.glScaled(scale, scale, scale);
		gl.glTranslated(offset.x, offset.y, 0.0);
		
		// render all the bodies in the world
		List<SimulationBody> list = new ArrayList<>();
		int bSize = world.getBodyCount();
		for (int i = 0; i < bSize; i++) {
			try {
				list.add((SimulationBody) world.getBody(i));
			}catch(Exception ex) {}
		}

		Collections.sort(list, (c1, c2) -> {
			if (c1.getLayer() > c2.getLayer()) return -1;
			else if (c1.getLayer() < c2.getLayer()) return 1;

			return 0;
		});

		for(SimulationBody b : list){

			this.renderBody(gl, b);

		}

		gl.glColor4fv(ColorUtilities.convertColor(new Color(0, 0, 0)), 0);
		RenderUtilities.fillShape(gl, new org.dyn4j.geometry.Rectangle(0.15f, 0.15f));

		gl.glPopMatrix();
	
	}
	
	/**
	 * Renders a body and related state normally.
	 * @param gl the OpenGL context
	 * @param body the body to render
	 */
	private void renderBody(GL2 gl, SimulationBody body) {
		// apply the transform
		RenderUtilities.pushTransform(gl);
		RenderUtilities.applyTransform(gl, body.getTransform());
		
		// render the body
		// check for multiple fixtures
		body.render(gl);
		
		// render the center of mass
		body.renderCenter(gl);

		RenderUtilities.popTransform(gl);
		
		// render the velocities
		body.renderVelocity(gl);
	}
	
	/**
	 * Updates the world.
	 */
	private void update() {

		try {
			executeQueue();

			// get the current time
			long time = System.nanoTime();
			// get the elapsed time from the last iteration
			long diff = time - this.last;
			// set the last time
			this.last = time;

			// check if the state is paused
			// convert from nanoseconds to seconds
			double elapsedTime = diff / NANO_TO_BASE;
			// update the world with the elapsed time
			world.update(elapsedTime);
			// see if its a compiled simulation
		}catch (Exception ex){ }
		
	}

    public class Mouse implements MouseWheelListener, com.jogamp.newt.event.MouseListener
    {

        private Vector2 start = null;
        private Vector2 begin = null;

        @Override
        public void mouseWheelMoved(MouseWheelEvent e)
        {

            if (e.getWheelRotation() < 0)
            {
                scale += 0.5;
            }
            else
            {
                scale -= 0.5;
            }

        }

		@Override
		public void mouseClicked(com.jogamp.newt.event.MouseEvent e) {

		}

		@Override
		public void mouseEntered(com.jogamp.newt.event.MouseEvent e) {

		}

		@Override
		public void mouseExited(com.jogamp.newt.event.MouseEvent e) {

		}

		@Override
		public void mousePressed(com.jogamp.newt.event.MouseEvent e) {
			canvas.setCursor(Cursor.getPredefinedCursor(Cursor.MOVE_CURSOR));
			start = offset.copy();
			begin = screenToWorld(new Point(e.getX(), e.getY()), offset, scale);
		}

		@Override
		public void mouseReleased(com.jogamp.newt.event.MouseEvent e) {
			canvas.setCursor(Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR));
			start = null;
		}

		@Override
		public void mouseMoved(com.jogamp.newt.event.MouseEvent e) {

		}

		@Override
		public void mouseDragged(com.jogamp.newt.event.MouseEvent e) {
			if (start == null || begin == null) return;

			canvas.setCursor(Cursor.getPredefinedCursor(Cursor.MOVE_CURSOR));
			Vector2 pwt = screenToWorld(new Point(e.getX(), e.getY()), start, scale);
			Vector2 tx = pwt.difference(begin);
			offset.add(tx);
			begin = pwt;
		}

		@Override
		public void mouseWheelMoved(com.jogamp.newt.event.MouseEvent e) {

		}
	}

    private Vector2 screenToWorld(Point p, Vector2 offset, double scale) {
        Vector2 v = new Vector2();
        double x = p.x;
        double y = p.y;
        double w = size.getWidth();
        double h = size.getHeight();
        double ox = offset.x;
        double oy = offset.y;
        
        v.x = (x - w * 0.5) / scale - ox;
        v.y = -((y - h * 0.5) / scale + oy);
        
        return v;
    }

    private List<Runnable> runnables = new ArrayList<>();

    public void addInQueue(Runnable r){

    	runnables.add(r);

	}

	private void executeQueue(){

    	for(Runnable r : runnables){

    		try{

    			r.run();

			}catch(Exception e){}

		}

		runnables.clear();

	}
	
	/**
	 * Creates game objects and adds them to the world.
	 */
	protected abstract void initializeWorld();
	
	/**
	 * Starts the simulation.
	 */
	public void run() {

		if (Main.DEBUG) {

			try {
				UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
			} catch (Exception e) {
				e.printStackTrace();
			}

			frame.setVisible(true);
		}

	}
}
