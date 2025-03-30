/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect } from "react";
import { useTodoGroupStore } from "./todoGroupStore";
import { Button, Form, Input, notification, Space, Select } from "antd";
import { TodoGroupService } from "./todoGroupService";

export const TodoGroupEdit = () => {
  const { editTodoGroup, setTodoGroupEdit, fetchTodoGroups } =
    useTodoGroupStore();
  const [form] = Form.useForm();

  useEffect(() => {
    if (editTodoGroup?.id) {
      form.setFieldsValue(editTodoGroup);
    }
  }, [editTodoGroup, form]);

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      values.id = editTodoGroup?.id;

      try {
        await TodoGroupService.updateTodoGroup(values);
        notification.success({ message: "Operation successful!" });
        setTodoGroupEdit(null);
        await fetchTodoGroups();
      } catch (error: any) {
        console.error("Error updating todo group:", error);
        notification.error({ message: "Failed to update todo group!" });
      }
    });
  };

  return (
    <div>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
        <Form.Item
          label="Todo Group Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo group name!" },
          ]}
        >
          <Input placeholder="Enter todo group name" />
        </Form.Item>

        <Form.Item
          label="Select Colour"
          name="colour"
          rules={[{ required: true, message: "Please select a colour!" }]}
        >
          <Select optionLabelProp="label">
            {TodoGroupService.getColours().map((colour) => (
              <Select.Option
                key={colour.value}
                value={colour.value}
                label={
                  <span style={{ display: "flex", alignItems: "center" }}>
                    <span
                      style={{
                        display: "inline-block",
                        width: 20,
                        height: 20,
                        backgroundColor: colour.value,
                        marginRight: 10,
                        borderRadius: "50%",
                      }}
                    />
                    {colour.label}
                  </span>
                }
              >
                <span style={{ display: "flex", alignItems: "center" }}>
                  <span
                    style={{
                      display: "inline-block",
                      width: 20,
                      height: 20,
                      backgroundColor: colour.value,
                      marginRight: 10,
                      borderRadius: "50%",
                    }}
                  />
                  {colour.label}
                </span>
              </Select.Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button type="default" onClick={() => setTodoGroupEdit(null)}>
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </div>
  );
};
