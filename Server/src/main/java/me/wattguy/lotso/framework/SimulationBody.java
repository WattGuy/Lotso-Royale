package me.wattguy.lotso.framework;

import java.awt.Color;
import java.awt.Graphics2D;
import java.awt.geom.AffineTransform;
import java.awt.geom.Ellipse2D;

import me.wattguy.lotso.enums.CircleType;
import me.wattguy.lotso.utils.ColorUtilities;
import me.wattguy.lotso.utils.RenderUtilities;

import org.dyn4j.dynamics.Body;
import org.dyn4j.dynamics.BodyFixture;
import org.dyn4j.geometry.Convex;
import org.dyn4j.geometry.Vector2;
import org.dyn4j.geometry.Wound;

import com.jogamp.opengl.GL;
import com.jogamp.opengl.GL2;

/**
 * Custom Body class to add drawing functionality.
 * @author William Bittle
 * @version 3.2.1
 * @since 3.0.0
 */
public class SimulationBody extends Body {
	/** The color of the object */
	private Color color = new Color(255, 255, 255);
	private int layer = 0;

	public int getLayer(){
		return layer;
	}

	public SimulationBody(){}

	public SimulationBody(CircleType t) {

		switch(t){

			case ZONE:
				color = new Color(135, 206, 250);
				layer = 3;
				break;

			case ITEM:
				color = new Color(126, 250, 107);
				layer = 2;
				break;

			case BULLET:
				color = new Color(210, 203, 0);
				layer = 1;
				break;

			case PLAYER:
				color = new Color(250, 72, 77);
				layer = 0;
				break;

		}

	}
	
	/**
	 * Constructor.
	 * @param color a set color
	 */
	public SimulationBody(Color color) {
		this.color = color;
	}
	
	/**
	 * Renders the body normally.
	 * <p>
	 * Uses the fill and outline colors to fill and outline each fixture in sequence.
	 * @param gl the OpenGL context
	 */
	public void render(GL2 gl) {
		// loop over all the fixtures
		int fSize = this.fixtures.size();
		for (int i = 0; i < fSize; i++) {
			BodyFixture bodyFixture = this.getFixture(i);
			Convex convex = bodyFixture.getShape();
			
			// render the fill
			this.setFillColor(gl);
			RenderUtilities.fillShape(gl, convex);
			// render the outline
			this.setOutlineColor(gl);
			RenderUtilities.drawShape(gl, convex, false);
		}
	}
	
	/**
	 * Sets the OpenGL color to the outline color of this body.
	 * @param gl the OpenGL context
	 */
	public void setOutlineColor(GL2 gl) {
		// check for inactive
		if (!this.isActive()) {
			float[] color = ColorUtilities.convertColor(this.color);
			gl.glColor4f(color[0] * 0.8f, color[1] * 0.8f, color[2] * 0.8f, color[3]);
		// check for asleep
		} else if (this.isAsleep()) {
			//float[] color = new float[] {0.78f, 0.78f, 1.0f, 1.0f};
			float[] color = ColorUtilities.convertColor(this.color.brighter());
	    	gl.glColor4f(color[0] * 0.8f, color[1] * 0.8f, color[2] * 0.8f, color[3]);
	    // otherwise just set it normally
	    } else {
	    	gl.glColor4fv(ColorUtilities.convertColor(color), 0);
	    }
	}
	
	/**
	 * Sets the OpenGL color to the fill color of this body.
	 * @param gl the OpenGL context
	 */
	public void setFillColor(GL2 gl) {
		// check for inactive
		if (!this.isActive()) {
			float[] color = ColorUtilities.convertColor(this.color);
	    	gl.glColor4fv(color, 0);
		// check for asleep
		} else if (this.isAsleep()) {
			float[] color = ColorUtilities.convertColor(this.color.brighter());
	    	gl.glColor4fv(color, 0);
    	// otherwise just set it normally
	    } else {
	    	gl.glColor4fv(ColorUtilities.convertColor(this.color), 0);
	    }
	}
	
	/**
	 * Renders the center of mass of this body.
	 * @param gl the OpenGL context
	 */
	public void renderCenter(GL2 gl) {
		renderCenter(gl, this.mass.getCenter());
	}

	public static void renderCenter(GL2 gl, Vector2 c){

		// set the color
		gl.glColor4fv(new float[] {0.0f, 0.0f, 0.0f, 1.0f}, 0);
		RenderUtilities.drawPoint(gl, c);

	}
	
	/**
	 * Renders the linear and angular velocity.
	 * @param gl the OpenGL context
	 */
	public void renderVelocity(GL2 gl) {
		// set the color
		gl.glColor4fv(new float[] {0.8f, 0.8f, 0.8f, 1.0f}, 0);
		// draw the velocities
		Vector2 c = this.getWorldCenter();
		Vector2 v = this.getLinearVelocity();
		double av = this.getAngularVelocity();
		
		// draw the linear velocity for each body
		gl.glBegin(GL.GL_LINES);
			gl.glVertex2d(c.x, c.y);
			gl.glVertex2d(c.x + v.x, c.y + v.y);
		gl.glEnd();
		
		// draw an arc
		RenderUtilities.drawArc(gl, c.x, c.y, 0.125, 0, av);
	}
	
	/**
	 * Returns this body's color.
	 * @return Color
	 */
	public Color getColor() {
		return this.color;
	}
	
	/**
	 * Sets the body's color
	 * @param color the color
	 */
	public void setColor(Color color) {
		this.color = color;
	}
}